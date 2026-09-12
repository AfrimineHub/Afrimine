using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Implementation
{
    public class BuyerService : IBuyerService
    {
        private readonly IRepositoryManager _repository;

        public BuyerService(IRepositoryManager repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<BuyerDashboardSummaryDto>> GetBuyerSummaryAsync(string buyerId)
        {
            var savedCount = await _repository.SavedListing.CountByUserIdAsync(buyerId);
            var unread = await _repository.Notification.CountUnreadAsync(buyerId);
            var ongoing = await _repository.Order.CountByUserAndStatusAsync(buyerId, OrderStatus.Ongoing);
            var openRfqs = await _repository.Rfq.CountOpenByBuyerAsync(buyerId);

            return ApiResponse<BuyerDashboardSummaryDto>.Ok(new BuyerDashboardSummaryDto
            {
                SavedListingsCount = savedCount,
                UnreadMessagesCount = unread,
                OngoingOrdersCount = ongoing,
                OpenRfqsCount = openRfqs
            });
        }

        public async Task<ApiResponse<PagedResultDto<MarketplaceListingDto>>> SearchListingsAsync(
            MarketplaceQueryDto query)
        {
            var (items, total) = await _repository.Listing.SearchMarketplaceAsync(
                query.Q, query.Location, query.Mineral,
                query.ListingType, query.VerifiedOnly,
                query.Page, query.PageSize);

            return ApiResponse<PagedResultDto<MarketplaceListingDto>>.Ok(new PagedResultDto<MarketplaceListingDto>
            {
                Items = items.Select(MapToMarketplaceDto),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<ApiResponse<MarketplaceListingDetailDto>> GetListingDetailAsync(
            Guid listingId, string? viewerId)
        {
            var listing = await _repository.Listing.GetByIdWithImagesAsync(listingId);
            if (listing is null)
                return ApiResponse<MarketplaceListingDetailDto>.Fail("Listing not found.", 404);

            if (viewerId is null || viewerId != listing.OwnerId)
            {
                listing.ViewsCount++;
                listing.UpdatedAt = DateTime.UtcNow;
                _repository.Listing.Update(listing);
                await _repository.SaveAsync();
            }

            return ApiResponse<MarketplaceListingDetailDto>.Ok(MapToMarketplaceDetailDto(listing));
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetCategoriesAsync()
        {
            var categories = await _repository.Listing.GetCategoriesAsync();
            return ApiResponse<IEnumerable<string>>.Ok(categories);
        }

        public async Task<ApiResponse<string>> InquireListingAsync(
            string buyerId, Guid listingId, CreateInquiryDto request)
        {
            var listing = await _repository.Listing.GetByIdAsync(listingId);
            if (listing is null)
                return ApiResponse<string>.Fail("Listing not found.", 404);
            if (listing.OwnerId == buyerId)
                return ApiResponse<string>.Fail("Cannot inquire on your own listing.", 400);

            await _repository.Inquiry.Create(new Inquiry
            {
                BuyerId = buyerId,
                VendorId = listing.OwnerId,
                ListingId = listingId,
                Message = request.Message
            });

            listing.InquiriesCount++;
            _repository.Listing.Update(listing);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Inquiry sent successfully.");
        }

        public async Task<ApiResponse<PagedResultDto<BuyerOrderDto>>> GetOrdersAsync(
            string buyerId, BuyerOrderQueryDto query)
        {
            var (items, total) = await _repository.Order.GetBuyerOrdersAsync(
                buyerId, query.Page, query.PageSize, query.Status);

            return ApiResponse<PagedResultDto<BuyerOrderDto>>.Ok(new PagedResultDto<BuyerOrderDto>
            {
                Items = items.Select(MapToBuyerOrderDto),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<ApiResponse<BuyerOrderDto>> GetOrderByIdAsync(string buyerId, Guid orderId)
        {
            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is null) return ApiResponse<BuyerOrderDto>.Fail("Order not found.", 404);
            if (order.BuyerId != buyerId) return ApiResponse<BuyerOrderDto>.Fail("Access denied.", 403);
            return ApiResponse<BuyerOrderDto>.Ok(MapToBuyerOrderDto(order));
        }

        public async Task<ApiResponse<string>> ConfirmDeliveryAsync(string buyerId, Guid orderId)
        {
            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is null) return ApiResponse<string>.Fail("Order not found.", 404);
            if (order.BuyerId != buyerId) return ApiResponse<string>.Fail("Access denied.", 403);
            if (order.Status != OrderStatus.Paid)
                return ApiResponse<string>.Fail("Order must be in Paid status to confirm delivery.", 409);

            order.Status = OrderStatus.Delivered;
            order.DeliveredAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            _repository.Order.Update(order);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Delivery confirmed.");
        }

        public async Task<ApiResponse<string>> DisputeOrderAsync(string buyerId, Guid orderId, DisputeOrderDto request)
        {
            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is null) return ApiResponse<string>.Fail("Order not found.", 404);
            if (order.BuyerId != buyerId) return ApiResponse<string>.Fail("Access denied.", 403);
            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                return ApiResponse<string>.Fail("Cannot dispute a completed or cancelled order.", 409);

            order.Status = OrderStatus.Disputed;
            order.DisputeReason = request.Reason;
            order.UpdatedAt = DateTime.UtcNow;
            _repository.Order.Update(order);

            await _repository.Dispute.Create(new Dispute
            {
                OrderId = order.Id,
                RaisedById = buyerId,
                Reason = request.Reason,
                Status = DisputeStatus.Open
            });

            var escrow = await _repository.Escrow.GetByOrderIdAsync(orderId);
            if (escrow is not null)
            {
                escrow.Status = EscrowStatus.Frozen;
                escrow.FrozenAt = DateTime.UtcNow;
                _repository.Escrow.Update(escrow);
            }

            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Dispute raised successfully.");
        }

        public async Task<ApiResponse<string>> PayOrderAsync(string buyerId, Guid orderId, PayOrderDto request)
        {
            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is null) return ApiResponse<string>.Fail("Order not found.", 404);
            if (order.BuyerId != buyerId) return ApiResponse<string>.Fail("Access denied.", 403);
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Ongoing)
                return ApiResponse<string>.Fail("Order is not in a payable state.", 409);

            order.Status = OrderStatus.Paid;
            order.PaymentReference = request.PaymentReference;
            order.PaidAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            _repository.Order.Update(order);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Payment recorded. Order is now in escrow.");
        }

        public async Task<ApiResponse<PagedResultDto<RfqDto>>> GetBuyerRfqsAsync(string buyerId, RfqQueryDto query)
        {
            var (items, total) = await _repository.Rfq.GetBuyerRfqsAsync(buyerId, query.Page, query.PageSize);

            var rfqList = items.ToList();

            var dtos = new List<RfqDto>();
            foreach (var rfq in rfqList)
            {
                var responseCount = await _repository.RfqQuote.CountByRfqIdAsync(rfq.Id);
                var dto = MapToRfqDto(rfq);
                dto.ResponseCount = responseCount;
                dtos.Add(dto);
            }

            return ApiResponse<PagedResultDto<RfqDto>>.Ok(new PagedResultDto<RfqDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<ApiResponse<RfqDto>> GetRfqByIdAsync(Guid rfqId)
        {
            var rfq = await _repository.Rfq.GetByIdAsync(rfqId);
            if (rfq is null) return ApiResponse<RfqDto>.Fail("RFQ not found.", 404);
            return ApiResponse<RfqDto>.Ok(MapToRfqDto(rfq));
        }

        public async Task<ApiResponse<RfqDto>> CreateRfqAsync(string buyerId, CreateRfqDto request)
        {
            var rfq = new Rfq
            {
                BuyerId = buyerId,
                Title = request.Title,
                Description = request.Description,
                MineralType = request.MineralType,
                Quantity = request.Quantity,
                Unit = request.Unit,
                TargetPrice = request.TargetPrice,
                Location = request.Location,
                Country = request.Country,
                ExpiresAt = request.ExpiresAt
            };

            await _repository.Rfq.Create(rfq);
            await _repository.SaveAsync();

            var created = await _repository.Rfq.GetByIdAsync(rfq.Id);
            return ApiResponse<RfqDto>.Ok(MapToRfqDto(created!), 201, "RFQ created successfully.");
        }

        // ── Mappers ───────────────────────────────────────────────────────────
        private static MarketplaceListingDto MapToMarketplaceDto(Listing l) => new()
        {
            Id = l.Id,
            Title = l.Title,
            Description = l.Description,
            Location = l.Location,
            Country = l.Country,
            Category = l.CategoryType.ToString(),
            CategoryLabel = GetCategoryLabel(l.CategoryType),
            PrimaryImageUrl = l.Images.FirstOrDefault(x => x.IsPrimary && !x.IsDeleted)?.ImageUrl
                              ?? l.Images.FirstOrDefault(x => !x.IsDeleted)?.ImageUrl,
            PriceAmount = l.PriceAmount,
            PriceCurrency = l.PriceCurrency,
            PriceUnit = l.PriceUnit,
            PriceDescription = l.PriceDescription,
            MineralType = l.MineralType,
            GradeOrPurity = l.GradeOrPurity,
            EquipmentType = l.EquipmentType,
            Condition = l.Condition,
            Quantity = l.Quantity,
            VendorName = l.Owner?.UserName ?? string.Empty,
            IsVerified = false,
            ViewsCount = l.ViewsCount,
            CreatedAt = l.CreatedAt
        };

        private static MarketplaceListingDetailDto MapToMarketplaceDetailDto(Listing l) => new()
        {
            Id = l.Id,
            Title = l.Title,
            Description = l.Description,
            Location = l.Location,
            Country = l.Country,
            Category = l.CategoryType.ToString(),
            CategoryLabel = GetCategoryLabel(l.CategoryType),
            PrimaryImageUrl = l.Images.FirstOrDefault(x => x.IsPrimary && !x.IsDeleted)?.ImageUrl
                              ?? l.Images.FirstOrDefault(x => !x.IsDeleted)?.ImageUrl,
            PriceAmount = l.PriceAmount,
            PriceCurrency = l.PriceCurrency,
            PriceUnit = l.PriceUnit,
            PriceDescription = l.PriceDescription,
            MineralType = l.MineralType,
            GradeOrPurity = l.GradeOrPurity,
            EquipmentType = l.EquipmentType,
            Condition = l.Condition,
            Quantity = l.Quantity,
            VendorName = l.Owner?.UserName ?? string.Empty,
            IsVerified = false,
            ViewsCount = l.ViewsCount,
            CreatedAt = l.CreatedAt,
            ContactInfo = l.ContactInfo,
            LeaseType = l.LeaseType,
            AcreageHectares = l.AcreageHectares,
            YearManufactured = l.YearManufactured,
            ManpowerRole = l.ManpowerRole,
            Availability = l.Availability,
            Images = l.Images.Where(x => !x.IsDeleted).Select(img => new ListingImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl,
                FileName = img.FileName,
                IsPrimary = img.IsPrimary
            }).ToList()
        };

        private static BuyerOrderDto MapToBuyerOrderDto(Order o) => new()
        {
            Id = o.Id,
            ListingId = o.ListingId,
            ListingTitle = o.Listing?.Title ?? string.Empty,
            VendorName = o.Vendor?.UserName ?? string.Empty,
            Amount = o.Amount,
            Currency = o.Currency,
            Status = o.Status.ToString().ToLower(),
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        };

        private static RfqDto MapToRfqDto(Rfq r) => new()
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            MineralType = r.MineralType,
            Quantity = r.Quantity,
            Unit = r.Unit,
            TargetPrice = r.TargetPrice,
            Location = r.Location,
            Country = r.Country,
            Status = r.Status.ToString(),
            BuyerName = r.Buyer?.UserName ?? string.Empty,
            ExpiresAt = r.ExpiresAt,
            CreatedAt = r.CreatedAt
        };

        private static string GetCategoryLabel(ListingCategory cat) => cat switch
        {
            ListingCategory.MiningSite => "Mining Site",
            ListingCategory.MineralSupply => "Mineral Supply",
            ListingCategory.Equipment => "Equipment",
            ListingCategory.Investment => "Investment",
            _ => cat.ToString()
        };
    }
}
