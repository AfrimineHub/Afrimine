using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── Users ──────────────────────────────────────────────────────────────
        public async Task<(IEnumerable<User> Items, int TotalCount)> GetUsersAsync(string? q, string? role, string? kycStatus, string? accountStatus, int page, int pageSize)
        {
            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.FullName.Contains(q) || x.Email!.Contains(q));

            if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<RoleType>(role, true, out var roleEnum))
                query = query.Where(x => x.Type == roleEnum);

            if (!string.IsNullOrWhiteSpace(accountStatus) && Enum.TryParse<AccountStatus>(accountStatus, true, out var statusEnum))
                query = query.Where(x => x.Status == statusEnum);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedOn)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<int> CountUsersAsync() => await _context.Users.CountAsync();

        public async Task<int> CountActiveUsersAsync() =>
            await _context.Users.CountAsync(x => x.Status == AccountStatus.Active);

        public async Task<int> CountKycVerifiedAsync() =>
            await _context.Set<VendorProfile>().CountAsync(x => x.KycStatus == KycStatus.Verified);

        public async Task<int> CountVendorsAsync() =>
            await _context.Users.CountAsync(x => x.Type == RoleType.Vendor);

        // ── Listings ───────────────────────────────────────────────────────────
        public async Task<(IEnumerable<Listing> Items, int TotalCount)> GetListingsAsync(string? status, string? q, int page, int pageSize)
        {
            IQueryable<Listing> query = _context.Set<Listing>()
                .Include(x => x.Owner)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Title.Contains(q) || x.Location.Contains(q));

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ListingStatus>(status, true, out var statusEnum))
                query = query.Where(x => x.Status == statusEnum);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<int> CountListingsByStatusAsync(ListingStatus? status) =>
            status.HasValue
                ? await _context.Set<Listing>().CountAsync(x => x.Status == status.Value && !x.IsDeleted)
                : await _context.Set<Listing>().CountAsync(x => !x.IsDeleted);

        // ── Quotes ─────────────────────────────────────────────────────────────
        public async Task<(IEnumerable<Quote> Items, int TotalCount)> GetQuotesAsync(string? q, string? status, int page, int pageSize)
        {
            IQueryable<Quote> query = _context.Set<Quote>()
                .Include(x => x.Buyer)
                .Include(x => x.Listing)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Listing.Title.Contains(q) || x.Buyer.FullName.Contains(q));

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<QuoteStatus>(status, true, out var statusEnum))
                query = query.Where(x => x.Status == statusEnum);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        // ── Orders ─────────────────────────────────────────────────────────────
        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetOrdersAsync(string? q, string? status, int page, int pageSize)
        {
            IQueryable<Order> query = _context.Set<Order>()
                .Include(x => x.Buyer)
                .Include(x => x.Vendor)
                .Include(x => x.Listing)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Buyer.FullName.Contains(q)
                    || x.Vendor.FullName.Contains(q)
                    || x.Listing.Title.Contains(q));

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var statusEnum))
                query = query.Where(x => x.Status == statusEnum);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<Order?> GetOrderDetailAsync(Guid orderId) =>
            await _context.Set<Order>()
                .Include(x => x.Buyer)
                .Include(x => x.Vendor)
                .Include(x => x.Listing)
                .FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted);

        public async Task<int> CountOrdersByStatusAsync(OrderStatus status) =>
            await _context.Set<Order>().CountAsync(x => x.Status == status && !x.IsDeleted);

        public async Task<int> CountAllOrdersAsync(string? q)
        {
            IQueryable<Order> query = _context.Set<Order>()
                .Include(x => x.Buyer).Include(x => x.Vendor)
                .Where(x => !x.IsDeleted);
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Buyer.FullName.Contains(q) || x.Vendor.FullName.Contains(q));
            return await query.CountAsync();
        }

        // ── Revenue ────────────────────────────────────────────────────────────
        public async Task<decimal> GetTotalRevenueAsync() =>
            await _context.Set<Revenue>().SumAsync(x => x.Amount);

        public async Task<decimal> GetTotalRevenueLastMonthAsync()
        {
            var last = DateTime.UtcNow.AddMonths(-1);
            return await _context.Set<Revenue>()
                .Where(x => x.TransactionDate.Year == last.Year && x.TransactionDate.Month == last.Month)
                .SumAsync(x => x.Amount);
        }

        public async Task<decimal> GetVendorPayoutsAsync() =>
            await _context.Set<Payout>()
                .Where(x => x.Status == PayoutStatus.Completed)
                .SumAsync(x => x.Amount);

        public async Task<decimal> GetVendorPayoutsLastMonthAsync()
        {
            var last = DateTime.UtcNow.AddMonths(-1);
            return await _context.Set<Payout>()
                .Where(x => x.Status == PayoutStatus.Completed
                    && x.CreatedAt.Year == last.Year && x.CreatedAt.Month == last.Month)
                .SumAsync(x => x.Amount);
        }

        public async Task<decimal> GetPendingPaymentsAsync() =>
            await _context.Set<Escrow>()
                .Where(x => x.Status == EscrowStatus.Funded)
                .SumAsync(x => x.Amount);

        public async Task<(IEnumerable<Revenue> Items, int TotalCount)> GetTransactionsAsync(string? status, int page, int pageSize)
        {
            IQueryable<Revenue> query = _context.Set<Revenue>()
                .Include(x => x.Vendor)
                .Include(x => x.Listing);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.TransactionDate)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        // ── Withdrawals ────────────────────────────────────────────────────────
        public async Task<(IEnumerable<Payout> Items, int TotalCount)> GetWithdrawalsAsync(string? q, string? status, int page, int pageSize)
        {
            IQueryable<Payout> query = _context.Set<Payout>()
                .Include(x => x.Vendor)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Vendor.FullName.Contains(q) || x.Vendor.Email!.Contains(q));

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PayoutStatus>(status, true, out var statusEnum))
                query = query.Where(x => x.Status == statusEnum);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<Payout?> GetPayoutByIdAsync(Guid payoutId) =>
            await _context.Set<Payout>()
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(x => x.Id == payoutId && !x.IsDeleted);

        // ── KYC ────────────────────────────────────────────────────────────────
        public async Task<(IEnumerable<VendorProfile> Items, int TotalCount)> GetKycQueueAsync(string? q, string? status, int page, int pageSize)
        {
            IQueryable<VendorProfile> query = _context.Set<VendorProfile>()
                .Include(x => x.User);

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<KycStatus>(status, true, out var kycEnum))
                query = query.Where(x => x.KycStatus == kycEnum);
            else
                query = query.Where(x => x.KycStatus == KycStatus.Pending);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.User.FullName.Contains(q) || x.User.Email!.Contains(q));

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<VendorProfile?> GetKycDetailAsync(Guid profileId) =>
            await _context.Set<VendorProfile>()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == profileId);

        // ── Dashboard helpers ──────────────────────────────────────────────────
        public async Task<int> CountOpenDisputesAsync() =>
            await _context.Set<Dispute>().CountAsync(x => x.Status == DisputeStatus.Open);

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count) =>
            await _context.Set<Order>()
                .Include(x => x.Buyer).Include(x => x.Vendor).Include(x => x.Listing)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt).Take(count).ToListAsync();

        public async Task<IEnumerable<VendorProfile>> GetRecentKycSubmissionsAsync(int count) =>
            await _context.Set<VendorProfile>()
                .Include(x => x.User)
                .Where(x => x.KycStatus == KycStatus.Pending)
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt).Take(count).ToListAsync();

        public async Task<IEnumerable<Dispute>> GetOpenDisputesAsync(int count) =>
            await _context.Set<Dispute>()
                .Include(x => x.Order).ThenInclude(o => o.Listing)
                .Include(x => x.RaisedBy)
                .Where(x => x.Status == DisputeStatus.Open)
                .OrderByDescending(x => x.CreatedAt).Take(count).ToListAsync();
    }
}