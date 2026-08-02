using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afrimine.Services.BL.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly IRepositoryManager _repository;
        private readonly IAdminRepository _admin;
        private readonly UserManager<Afrimine.Model.Entities.User> _userManager;

        public AdminService(
            IRepositoryManager repository,
            IAdminRepository admin,
            UserManager<Afrimine.Model.Entities.User> userManager)
        {
            _repository = repository;
            _admin = admin;
            _userManager = userManager;
        }

        // ── Dashboard ─────────────────────────────────────────────────────────
        public async Task<ApiResponse<AdminDashboardDto>> GetDashboardAsync()
        {
            var totalUsers = await _admin.CountUsersAsync();
            var activeUsers = await _admin.CountActiveUsersAsync();
            var kycVerified = await _admin.CountKycVerifiedAsync();
            var vendors = await _admin.CountVendorsAsync();
            var totalRevenue = await _admin.GetTotalRevenueAsync();
            var pendingPayments = await _admin.GetPendingPaymentsAsync();
            var openDisputes = await _admin.CountOpenDisputesAsync();
            var pendingListings = await _admin.CountListingsByStatusAsync(ListingStatus.PendingReview);

            var recentOrders = await _admin.GetRecentOrdersAsync(5);
            var recentKyc = await _admin.GetRecentKycSubmissionsAsync(3);
            var disputes = await _admin.GetOpenDisputesAsync(3);

            var stats = new List<AdminStatItemDto>
            {
                new() { Id = "total_users", Title = "Total Users", Value = totalUsers, IsNeutral = true },
                new() { Id = "active_users", Title = "Active Users", Value = activeUsers, IsPositive = true },
                new() { Id = "kyc_verified", Title = "KYC Verified", Value = kycVerified, IsPositive = true },
                new() { Id = "vendors", Title = "Total Vendors", Value = vendors, IsNeutral = true },
                new() { Id = "total_revenue", Title = "Total Revenue", Value = totalRevenue, IsPositive = true },
                new() { Id = "pending_payments", Title = "Pending Payments", Value = pendingPayments, IsNeutral = true },
                new() { Id = "open_disputes", Title = "Open Disputes", Value = openDisputes, IsPositive = openDisputes == 0 },
                new() { Id = "pending_listings", Title = "Pending Listings", Value = pendingListings, IsNeutral = true }
            };

            var alerts = new List<AdminAlertDto>();
            foreach (var d in disputes)
            {
                alerts.Add(new AdminAlertDto
                {
                    Id = d.Id.ToString(),
                    Type = "danger",
                    Title = "Open Dispute",
                    Description = $"{d.RaisedBy?.FullName ?? "User"} raised a dispute: {d.Reason[..Math.Min(60, d.Reason.Length)]}",
                    Time = TimeAgo(d.CreatedAt),
                    ActionText = "Review",
                    ActionUrl = $"/admin/dispute/{d.Id}"
                });
            }
            foreach (var k in recentKyc)
            {
                alerts.Add(new AdminAlertDto
                {
                    Id = k.Id.ToString(),
                    Type = "warning",
                    Title = "KYC Pending Review",
                    Description = $"{k.User?.FullName ?? "Vendor"} submitted KYC documents",
                    Time = TimeAgo(k.CreatedAt),
                    ActionText = "Review",
                    ActionUrl = $"/admin/kyc/{k.Id}"
                });
            }

            var activity = recentOrders.Select(o => new AdminActivityDto
            {
                Id = o.Id.ToString(),
                Type = o.Status == OrderStatus.Completed ? "success"
                     : o.Status == OrderStatus.Disputed ? "danger" : "info",
                Title = $"Order {o.Status}",
                Description = $"{o.Buyer?.FullName ?? "Buyer"} → {o.Listing?.Title ?? "Listing"} ({o.Currency} {o.Amount:N0})",
                CreatedAt = o.CreatedAt.ToString("O")
            }).ToList();

            var ongoing = recentOrders
                .Where(o => o.Status == OrderStatus.Ongoing || o.Status == OrderStatus.Paid)
                .Select(o => new AdminOngoingTransactionDto
                {
                    Id = o.Id.ToString(),
                    OrderId = o.Id.ToString(),
                    BuyerName = o.Buyer?.FullName,
                    VendorName = o.Vendor?.FullName,
                    Amount = o.Amount,
                    Currency = o.Currency,
                    Status = o.Status.ToString()
                }).ToList();

            return ApiResponse<AdminDashboardDto>.Ok(new AdminDashboardDto
            {
                Stats = stats,
                PriorityAlerts = alerts,
                RecentActivity = activity,
                OngoingTransactions = ongoing
            });
        }

        // ── Users ─────────────────────────────────────────────────────────────
        public async Task<ApiResponse<PagedResultDto<AdminUserListItemDto>>> GetUsersAsync(AdminUserQueryDto query)
        {
            var (items, total) = await _admin.GetUsersAsync(
                query.Q, query.Role, query.KycStatus, query.AccountStatus, query.Page, query.PageSize);

            var userIds = items.Select(u => Guid.Parse(u.Id)).ToList();
            var profiles = await _repository.VendorProfile.GetByUserIdsAsync(userIds);
            var profileMap = profiles.ToDictionary(x => x.UserId, x => x);

            return ApiResponse<PagedResultDto<AdminUserListItemDto>>.Ok(new PagedResultDto<AdminUserListItemDto>
            {
                Items = items.Select(u =>
                {
                    profileMap.TryGetValue(u.Id, out var profile);
                    return new AdminUserListItemDto
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        Role = u.Type.ToString().ToLower(),
                        KycStatus = profile?.KycStatus.ToString().ToLower() ?? "not_started",
                        AccountStatus = u.Status.ToString().ToLower(),
                        CreatedAt = u.CreatedOn.ToString("O")
                    };
                }),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<ApiResponse<AdminUserStatsDto>> GetUserStatsAsync()
        {
            return ApiResponse<AdminUserStatsDto>.Ok(new AdminUserStatsDto
            {
                TotalUsers = await _admin.CountUsersAsync(),
                ActiveUsers = await _admin.CountActiveUsersAsync(),
                KycVerified = await _admin.CountKycVerifiedAsync(),
                Vendors = await _admin.CountVendorsAsync()
            });
        }

        public async Task<ApiResponse<string>> SuspendUserAsync(string userId, AdminUserActionDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return ApiResponse<string>.Fail("User not found.", 404);
            user.Status = AccountStatus.Suspended;
            user.SuspendedReason = request.Reason;
            await _userManager.UpdateAsync(user);
            return ApiResponse<string>.Ok("User suspended.");
        }

        public async Task<ApiResponse<string>> BanUserAsync(string userId, AdminUserActionDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return ApiResponse<string>.Fail("User not found.", 404);
            user.Status = AccountStatus.Banned;
            user.BannedReason = request.Reason;
            await _userManager.UpdateAsync(user);
            return ApiResponse<string>.Ok("User banned.");
        }

        public async Task<ApiResponse<string>> ReactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return ApiResponse<string>.Fail("User not found.", 404);
            user.Status = AccountStatus.Active;
            user.SuspendedReason = null;
            user.BannedReason = null;
            await _userManager.UpdateAsync(user);
            return ApiResponse<string>.Ok("User reactivated.");
        }

        // ── Listings ──────────────────────────────────────────────────────────
        public async Task<ApiResponse<PagedResultDto<AdminListingListItemDto>>> GetListingsAsync(
            AdminListingQueryDto query)
        {
            var (items, total) = await _admin.GetListingsAsync(query.Status, query.Q, query.Page, query.PageSize);
            return ApiResponse<PagedResultDto<AdminListingListItemDto>>.Ok(new PagedResultDto<AdminListingListItemDto>
            {
                Items = items.Select(l => new AdminListingListItemDto
                {
                    Id = l.Id.ToString(),
                    Title = l.Title,
                    Category = l.CategoryType.ToString(),
                    Location = l.Location,
                    SellerName = l.Owner?.FullName,
                    Price = string.IsNullOrWhiteSpace(l.PriceDescription)
                        ? $"{l.PriceCurrency} {l.PriceAmount:N0}"
                        : l.PriceDescription,
                    PriceAmount = l.PriceAmount,
                    Currency = l.PriceCurrency,
                    Status = l.Status.ToString().ToLower(),
                    CreatedAt = l.CreatedAt.ToString("O")
                }),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<ApiResponse<AdminListingCountsDto>> GetListingCountsAsync()
        {
            return ApiResponse<AdminListingCountsDto>.Ok(new AdminListingCountsDto
            {
                All = await _admin.CountListingsByStatusAsync(null),
                Pending = await _admin.CountListingsByStatusAsync(ListingStatus.PendingReview),
                Approved = await _admin.CountListingsByStatusAsync(ListingStatus.Active),
                Rejected = await _admin.CountListingsByStatusAsync(ListingStatus.Rejected),
                Flagged = await _admin.CountListingsByStatusAsync(ListingStatus.Flagged)
            });
        }

        public async Task<ApiResponse<string>> ApproveListingAsync(Guid listingId)
        {
            var listing = await _repository.Listing.GetByIdAsync(listingId);
            if (listing is null) return ApiResponse<string>.Fail("Listing not found.", 404);
            listing.Status = ListingStatus.Active;
            listing.AdminReviewNote = null;
            listing.UpdatedAt = DateTime.UtcNow;
            _repository.Listing.Update(listing);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Listing approved.");
        }

        public async Task<ApiResponse<string>> RejectListingAsync(Guid listingId, AdminListingActionDto request)
        {
            var listing = await _repository.Listing.GetByIdAsync(listingId);
            if (listing is null) return ApiResponse<string>.Fail("Listing not found.", 404);
            listing.Status = ListingStatus.Rejected;
            listing.AdminReviewNote = request.Reason;
            listing.UpdatedAt = DateTime.UtcNow;
            _repository.Listing.Update(listing);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Listing rejected.");
        }

        public async Task<ApiResponse<string>> FlagListingAsync(Guid listingId, AdminListingActionDto request)
        {
            var listing = await _repository.Listing.GetByIdAsync(listingId);
            if (listing is null) return ApiResponse<string>.Fail("Listing not found.", 404);
            listing.Status = ListingStatus.Flagged;
            listing.AdminReviewNote = request.Reason;
            listing.UpdatedAt = DateTime.UtcNow;
            _repository.Listing.Update(listing);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Listing flagged.");
        }

        public async Task<ApiResponse<string>> DeleteListingAsync(Guid listingId)
        {
            var listing = await _repository.Listing.GetByIdAsync(listingId);
            if (listing is null) return ApiResponse<string>.Fail("Listing not found.", 404);
            listing.IsDeleted = true;
            listing.Status = ListingStatus.Archived;
            listing.UpdatedAt = DateTime.UtcNow;
            _repository.Listing.Update(listing);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Listing archived.");
        }

        // ── Quotes ────────────────────────────────────────────────────────────
        public async Task<ApiResponse<PagedResultDto<AdminQuoteListItemDto>>> GetQuotesAsync(AdminQuoteQueryDto query)
        {
            var (items, total) = await _admin.GetQuotesAsync(query.Q, query.Status, query.Page, query.PageSize);
            return ApiResponse<PagedResultDto<AdminQuoteListItemDto>>.Ok(new PagedResultDto<AdminQuoteListItemDto>
            {
                Items = items.Select(q => new AdminQuoteListItemDto
                {
                    Id = q.Id.ToString(),
                    ClientName = q.Buyer?.FullName,
                    CompanyName = null,
                    ProductName = q.Listing?.Title,
                    Quantity = null,
                    Status = q.Status.ToString().ToLower(),
                    CreatedAt = q.CreatedAt.ToString("O"),
                    IsUnread = false
                }),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        // ── Orders ────────────────────────────────────────────────────────────
        public async Task<ApiResponse<PagedResultDto<AdminOrderListItemDto>>> GetOrdersAsync(AdminOrderQueryDto query)
        {
            var (items, total) = await _admin.GetOrdersAsync(query.Q, query.Status, query.Page, query.PageSize);
            return ApiResponse<PagedResultDto<AdminOrderListItemDto>>.Ok(new PagedResultDto<AdminOrderListItemDto>
            {
                Items = items.Select(MapToOrderListItem),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<ApiResponse<AdminOrderSummaryDto>> GetOrderSummaryAsync(string? q)
        {
            return ApiResponse<AdminOrderSummaryDto>.Ok(new AdminOrderSummaryDto
            {
                Total = await _admin.CountAllOrdersAsync(q),
                Completed = await _admin.CountOrdersByStatusAsync(OrderStatus.Completed),
                InProgress = await _admin.CountOrdersByStatusAsync(OrderStatus.Ongoing),
                Pending = await _admin.CountOrdersByStatusAsync(OrderStatus.Pending),
                FailedOrCancelled = await _admin.CountOrdersByStatusAsync(OrderStatus.Cancelled)
            });
        }

        public async Task<ApiResponse<AdminOrderDetailDto>> GetOrderDetailAsync(Guid orderId)
        {
            var order = await _admin.GetOrderDetailAsync(orderId);
            if (order is null) return ApiResponse<AdminOrderDetailDto>.Fail("Order not found.", 404);

            return ApiResponse<AdminOrderDetailDto>.Ok(new AdminOrderDetailDto
            {
                Id = order.Id.ToString(),
                ListingTitle = order.Listing?.Title,
                Description = order.Listing?.Description,
                BuyerName = order.Buyer?.FullName,
                BuyerEmail = order.Buyer?.Email,
                VendorName = order.Vendor?.FullName,
                VendorEmail = order.Vendor?.Email,
                Amount = order.Amount,
                Currency = order.Currency,
                Status = order.Status.ToString().ToLower(),
                CreatedAt = order.CreatedAt.ToString("O"),
                Timeline = BuildTimeline(order),
                Documents = new List<AdminOrderDocumentDto>()
            });
        }

        // ── Revenue ───────────────────────────────────────────────────────────
        public async Task<ApiResponse<AdminRevenueSummaryDto>> GetRevenueSummaryAsync()
        {
            var total = await _admin.GetTotalRevenueAsync();
            var lastMonth = await _admin.GetTotalRevenueLastMonthAsync();
            var payouts = await _admin.GetVendorPayoutsAsync();
            var payoutsLastMonth = await _admin.GetVendorPayoutsLastMonthAsync();
            var pending = await _admin.GetPendingPaymentsAsync();

            return ApiResponse<AdminRevenueSummaryDto>.Ok(new AdminRevenueSummaryDto
            {
                TotalPlatformRevenue = total,
                TotalPlatformRevenueChangePercent = lastMonth > 0
                    ? Math.Round((double)((total - lastMonth) / lastMonth) * 100, 1) : null,
                VendorPayouts = payouts,
                VendorPayoutsChangePercent = payoutsLastMonth > 0
                    ? Math.Round((double)((payouts - payoutsLastMonth) / payoutsLastMonth) * 100, 1) : null,
                PendingPayments = pending,
                PendingPaymentsChangePercent = null,
                Currency = "USD"
            });
        }

        public async Task<ApiResponse<PagedResultDto<AdminTransactionItemDto>>> GetTransactionsAsync(
            AdminTransactionQueryDto query)
        {
            var (items, total) = await _admin.GetTransactionsAsync(query.Status, query.Page, query.PageSize);
            return ApiResponse<PagedResultDto<AdminTransactionItemDto>>.Ok(new PagedResultDto<AdminTransactionItemDto>
            {
                Items = items.Select(r => new AdminTransactionItemDto
                {
                    Id = r.Id.ToString(),
                    VendorName = r.Vendor?.FullName,
                    ProductName = r.Listing?.Title,
                    Amount = r.Amount,
                    Currency = r.Currency,
                    Status = "completed",
                    CreatedAt = r.CreatedAt.ToString("O")
                }),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        // ── Withdrawals ───────────────────────────────────────────────────────
        public async Task<ApiResponse<PagedResultDto<AdminWithdrawalItemDto>>> GetWithdrawalsAsync(
            AdminWithdrawalQueryDto query)
        {
            var (items, total) = await _admin.GetWithdrawalsAsync(query.Q, query.Status, query.Page, query.PageSize);

            //var vendorIds = items.Select(x => Guid.Parse(x.VendorId)).Distinct().ToList();
            var vendorIds = items.Where(x => !string.IsNullOrWhiteSpace(x.VendorId))
                .Select(x => Guid.Parse(x.VendorId))
                .Distinct()
                .ToList();

            var profiles = await _repository.VendorProfile.GetByUserIdsAsync(vendorIds);
            var profileMap = profiles.ToDictionary(x => x.UserId, x => x);

            return ApiResponse<PagedResultDto<AdminWithdrawalItemDto>>.Ok(new PagedResultDto<AdminWithdrawalItemDto>
            {
                Items = items.Select(p =>
                {
                    profileMap.TryGetValue(p.VendorId, out var profile);
                    return new AdminWithdrawalItemDto
                    {
                        Id = p.Id.ToString(),
                        VendorId = p.VendorId,
                        VendorName = p.Vendor?.FullName,
                        VendorEmail = p.Vendor?.Email,
                        Amount = p.Amount,
                        Currency = p.Currency,
                        BankName = profile?.BankName,
                        Status = p.Status.ToString().ToLower(),
                        RequestedAt = p.CreatedAt.ToString("O")
                    };
                }),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<ApiResponse<string>> ApproveWithdrawalAsync(Guid payoutId)
        {
            var payout = await _admin.GetPayoutByIdAsync(payoutId);
            if (payout is null) return ApiResponse<string>.Fail("Withdrawal not found.", 404);
            payout.Status = PayoutStatus.Completed;
            payout.ProcessedAt = DateTime.UtcNow;
            _repository.Payout.Update(payout);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Withdrawal approved.");
        }

        public async Task<ApiResponse<string>> HoldWithdrawalAsync(Guid payoutId, AdminWithdrawalActionDto request)
        {
            var payout = await _admin.GetPayoutByIdAsync(payoutId);
            if (payout is null) return ApiResponse<string>.Fail("Withdrawal not found.", 404);
            payout.Status = PayoutStatus.Processing;
            payout.Reference = request.Reason;
            _repository.Payout.Update(payout);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Withdrawal placed on hold.");
        }

        public async Task<ApiResponse<string>> RejectWithdrawalAsync(Guid payoutId, AdminWithdrawalActionDto request)
        {
            var payout = await _admin.GetPayoutByIdAsync(payoutId);
            if (payout is null) return ApiResponse<string>.Fail("Withdrawal not found.", 404);
            payout.Status = PayoutStatus.Failed;
            payout.Reference = request.Reason;
            _repository.Payout.Update(payout);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Withdrawal rejected.");
        }

        // ── KYC ───────────────────────────────────────────────────────────────
        public async Task<ApiResponse<PagedResultDto<AdminKycQueueItemDto>>> GetKycQueueAsync(AdminKycQueryDto query)
        {
            var (items, total) = await _admin.GetKycQueueAsync(query.Q, query.Status, query.Page, query.PageSize);
            return ApiResponse<PagedResultDto<AdminKycQueueItemDto>>.Ok(new PagedResultDto<AdminKycQueueItemDto>
            {
                Items = items.Select(p => new AdminKycQueueItemDto
                {
                    Id = p.Id.ToString(),
                    UserId = p.UserId,
                    FullName = p.User?.FullName,
                    Email = p.User?.Email,
                    Phone = p.User?.PhoneNumber,
                    DocumentType = p.DocumentType?.ToString(),
                    Country = p.Country,
                    SubmittedAt = (p.CreatedAt).ToString("O"),
                    Status = p.KycStatus.ToString().ToLower()
                }),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<ApiResponse<AdminKycDetailDto>> GetKycDetailAsync(Guid profileId)
        {
            var p = await _admin.GetKycDetailAsync(profileId);
            if (p is null) return ApiResponse<AdminKycDetailDto>.Fail("KYC submission not found.", 404);

            return ApiResponse<AdminKycDetailDto>.Ok(new AdminKycDetailDto
            {
                Id = p.Id.ToString(),
                UserId = p.UserId,
                FullName = p.User?.FullName,
                DateOfBirth = p.DateOfBirth,
                Email = p.User?.Email,
                Phone = p.User?.PhoneNumber,
                Address = p.OfficeAddress,
                DocumentType = p.DocumentType?.ToString(),
                DocumentIdNumber = p.DocumentIdNumber,
                Country = p.Country,
                SubmittedAt = (p.CreatedAt).ToString("O"),
                ProfilePhotoUrl = p.ProfilePhotoUrl,
                DocumentFileName = p.DocumentFileName,
                DocumentFileSizeBytes = p.DocumentFileSizeBytes,
                DocumentDownloadUrl = p.DocumentUrl,
                Status = p.KycStatus.ToString().ToLower(),
                RejectionReason = p.KycRejectionReason
            });
        }

        public async Task<ApiResponse<string>> ApproveKycAsync(Guid profileId)
        {
            var profile = await _admin.GetKycDetailAsync(profileId);
            if (profile is null) return ApiResponse<string>.Fail("KYC not found.", 404);
            profile.KycStatus = KycStatus.Verified;
            profile.KycRejectionReason = null;
            profile.UpdatedAt = DateTime.UtcNow;
            _repository.VendorProfile.Update(profile);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("KYC approved.");
        }

        public async Task<ApiResponse<string>> RejectKycAsync(Guid profileId, AdminKycActionDto request)
        {
            var profile = await _admin.GetKycDetailAsync(profileId);
            if (profile is null) return ApiResponse<string>.Fail("KYC not found.", 404);
            profile.KycStatus = KycStatus.Rejected;
            profile.KycRejectionReason = request.Reason;
            profile.UpdatedAt = DateTime.UtcNow;
            _repository.VendorProfile.Update(profile);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("KYC rejected.");
        }

        public async Task<ApiResponse<string>> CreateAdminAsync(CreateAdminDto request)
        {
            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing is not null)
                return ApiResponse<string>.Fail("Email already registered.", 409);

            var admin = new User
            {
                FullName = request.FullName,
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = "",
                EmailConfirmed = true,
                Type = RoleType.SuperAdmin,
                Status = AccountStatus.Active
            };

            var result = await _userManager.CreateAsync(admin, request.Password);
            if (!result.Succeeded)
                return ApiResponse<string>.Fail(
                    string.Join(", ", result.Errors.Select(e => e.Description)), 400);

            await _userManager.AddToRoleAsync(admin, "SuperAdmin");
            return ApiResponse<string>.Ok("Admin created successfully.");
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private static AdminOrderListItemDto MapToOrderListItem(Afrimine.Model.Entities.Order o) => new()
        {
            Id = o.Id.ToString(),
            ListingTitle = o.Listing?.Title,
            Description = o.Listing?.Description,
            BuyerName = o.Buyer?.FullName,
            BuyerEmail = o.Buyer?.Email,
            VendorName = o.Vendor?.FullName,
            VendorEmail = o.Vendor?.Email,
            Amount = o.Amount,
            Currency = o.Currency,
            Status = o.Status.ToString().ToLower(),
            CreatedAt = o.CreatedAt.ToString("O")
        };

        private static List<AdminOrderTimelineItemDto> BuildTimeline(Afrimine.Model.Entities.Order o)
        {
            var now = DateTime.UtcNow;
            return new List<AdminOrderTimelineItemDto>
            {
                new() { Step = "Order Placed",
                    OccurredAt = o.CreatedAt.ToString("O"),
                    Status = "completed" },
                new() { Step = "Payment", OccurredAt = (o.PaidAt ?? now).ToString("O"),
                    Status = o.PaidAt.HasValue ? "completed" : o.Status == OrderStatus.Pending ? "current" : "pending" },
                new() { Step = "In Escrow", OccurredAt = (o.PaidAt ?? now).ToString("O"),
                    Status = o.Status >= OrderStatus.Paid ? "completed" : o.Status == OrderStatus.Paid ? "current" : "pending" },
                new() { Step = "Delivered", OccurredAt = (o.DeliveredAt ?? now).ToString("O"),
                    Status = o.DeliveredAt.HasValue ? "completed" : o.Status == OrderStatus.Delivered ? "current" : "pending" },
                new() { Step = "Completed", OccurredAt = now.ToString("O"),
                    Status = o.Status == OrderStatus.Completed ? "completed" : o.Status == OrderStatus.Delivered ? "current" : "pending" }
            };
        }

        private static string TimeAgo(DateTime dt)
        {
            var diff = DateTime.UtcNow - dt;
            if (diff.TotalMinutes < 1) return "just now";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} min ago";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}hr ago";
            return $"{(int)diff.TotalDays}d ago";
        }
    }
}
