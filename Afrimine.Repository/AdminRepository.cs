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
            await _context.Set<SupplierProfile>().CountAsync(x => x.Status == SupplierStatus.Active);

        public async Task<int> CountVendorsAsync() =>
            await _context.Users.CountAsync(x => x.Type == RoleType.Vendor);

        // ── Listings ───────────────────────────────────────────────────────────
        public async Task<(IEnumerable<Listing> Items, int TotalCount)> GetListingsAsync(string? status, string? q, Guid? supplierId, int page, int pageSize)
        {
            // Only ever return listings owned by a Vendor/supplier account — this is the
            // "supplier listings" refactor: guards against orphaned or non-vendor owners.
            IQueryable<Listing> query = _context.Set<Listing>()
                .Include(x => x.Owner)
                .Where(x => !x.IsDeleted && x.Owner.Type == RoleType.Vendor);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Title.Contains(q) || x.Location.Contains(q));

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ListingStatus>(status, true, out var statusEnum))
                query = query.Where(x => x.Status == statusEnum);

            if (supplierId.HasValue)
            {
                var supplier = await _context.Set<SupplierProfile>()
                    .FirstOrDefaultAsync(x => x.Id == supplierId.Value);
                if (supplier != null)
                    query = query.Where(x => x.OwnerId == supplier.UserId);
            }

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<Dictionary<string, SupplierProfile>> GetSupplierProfilesByOwnerIdsAsync(IEnumerable<string> ownerIds)
        {
            var ids = ownerIds.Distinct().ToList();
            return await _context.Set<SupplierProfile>()
                .Where(x => ids.Contains(x.UserId))
                .GroupBy(x => x.UserId)
                .Select(g => g.OrderByDescending(x => x.CreatedAt).First())
                .ToDictionaryAsync(x => x.UserId, x => x);
        }

        public async Task<int> CountListingsByStatusAsync(ListingStatus? status) => status.HasValue
                ? await _context.Set<Listing>().CountAsync(x => x.Status == status.Value && !x.IsDeleted)
                : await _context.Set<Listing>().CountAsync(x => !x.IsDeleted);

    //    public async Task<int> CountListingsByStatusAsync(ListingStatus? status) =>
    //status.HasValue
    //    ? await _context.Set<Listing>().CountAsync(x => x.Status == status.Value && !x.IsDeleted && x.Owner.Type == RoleType.Vendor)
    //    : await _context.Set<Listing>().CountAsync(x => !x.IsDeleted && x.Owner.Type == RoleType.Vendor);

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
        public async Task<(IEnumerable<SupplierProfile> Items, int TotalCount)> GetKycQueueAsync(string? q, string? status, int page, int pageSize)
        {
            IQueryable<SupplierProfile> query = _context.Set<SupplierProfile>()
                .Include(x => x.User);

            if (!string.IsNullOrWhiteSpace(status)
                && Enum.TryParse<SupplierStatus>(status, true, out var statusEnum))
            {
                query = query.Where(x => x.Status == statusEnum);
            }
            else
            {
                query = query.Where(x => x.Status == SupplierStatus.Pending && !x.IsDeleted);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(x =>
                    (x.User.FullName != null && x.User.FullName.Contains(q)) ||
                    (x.User.Email != null && x.User.Email.Contains(q)) ||
                    (x.CompanyName != null && x.CompanyName.Contains(q)));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<SupplierProfile?> GetKycDetailAsync(Guid profileId) =>
            await _context.Set<SupplierProfile>()
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

        public async Task<IEnumerable<SupplierProfile>> GetRecentKycSubmissionsAsync(int count) =>
            await _context.Set<SupplierProfile>()
                .Include(x => x.User)
                .Where(x => x.Status == SupplierStatus.Pending)
                .OrderByDescending(x => x.CreatedAt).Take(count).ToListAsync();

        public async Task<IEnumerable<Dispute>> GetOpenDisputesAsync(int count) =>
            await _context.Set<Dispute>()
                .Include(x => x.Order).ThenInclude(o => o.Listing)
                .Include(x => x.RaisedBy)
                .Where(x => x.Status == DisputeStatus.Open)
                .OrderByDescending(x => x.CreatedAt).Take(count).ToListAsync();

        public async Task<(IEnumerable<Escrow> Items, int TotalCount)> GetEscrowPaymentsAsync(string? status, int page, int pageSize)
        {
            IQueryable<Escrow> query = _context.Set<Escrow>()
                .Include(x => x.Order).ThenInclude(o => o.Buyer)
                .Include(x => x.Order).ThenInclude(o => o.Vendor);

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<EscrowStatus>(status, true, out var statusEnum))
                query = query.Where(x => x.Status == statusEnum);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<bool> HardDeleteUserCascadeAsync(string userId)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    // ── 1. Conversations & messages (Buyer/Vendor side) ──
                    var convIds = await _context.Set<Conversation>()
                        .Where(c => c.BuyerId == userId || c.VendorId == userId)
                        .Select(c => c.Id).ToListAsync();
                    _context.RemoveRange(_context.Set<Message>().Where(m => convIds.Contains(m.ConversationId)));
                    _context.RemoveRange(_context.Set<Message>().Where(m => m.SenderId == userId));
                    await _context.SaveChangesAsync();
                    _context.RemoveRange(_context.Set<Conversation>().Where(c => convIds.Contains(c.Id)));
                    await _context.SaveChangesAsync();

                    // ── 2. Disputes & escrow tied to this user's orders ──
                    var orderIds = await _context.Set<Order>()
                        .Where(o => o.BuyerId == userId || o.VendorId == userId)
                        .Select(o => o.Id).ToListAsync();
                    _context.RemoveRange(_context.Set<Dispute>().Where(d => orderIds.Contains(d.OrderId) || d.RaisedById == userId));
                    _context.RemoveRange(_context.Set<Escrow>().Where(e => orderIds.Contains(e.OrderId) || e.BuyerId == userId || e.VendorId == userId));
                    await _context.SaveChangesAsync();

                    // ── 3. Bookings (as miner, or as supplier via SupplierProfile) + their children ──
                    var supplierProfile = await _context.Set<SupplierProfile>().FirstOrDefaultAsync(s => s.UserId == userId);
                    var bookingIds = await _context.Set<Booking>()
                        .Where(b => b.MinerId == userId || (supplierProfile != null && b.SupplierId == supplierProfile.Id))
                        .Select(b => b.Id).ToListAsync();
                    _context.RemoveRange(_context.Set<BookingDispute>().Where(bd => bookingIds.Contains(bd.BookingId) || bd.RaisedById == userId));
                    _context.RemoveRange(_context.Set<DailyCheck>().Where(dc => bookingIds.Contains(dc.BookingId)));
                    await _context.SaveChangesAsync();
                    _context.RemoveRange(_context.Set<Booking>().Where(b => bookingIds.Contains(b.Id)));
                    await _context.SaveChangesAsync();

                    // ── 4. RFQs / Quotes / Inquiries this user raised or received ──
                    var rfqIds = await _context.Set<Rfq>().Where(r => r.BuyerId == userId).Select(r => r.Id).ToListAsync();
                    _context.RemoveRange(_context.Set<RfqQuote>().Where(rq => rfqIds.Contains(rq.RfqId) || rq.VendorId == userId));
                    await _context.SaveChangesAsync();
                    _context.RemoveRange(_context.Set<Rfq>().Where(r => rfqIds.Contains(r.Id)));
                    _context.RemoveRange(_context.Set<Quote>().Where(q => q.BuyerId == userId || q.VendorId == userId));
                    _context.RemoveRange(_context.Set<Inquiry>().Where(i => i.BuyerId == userId || i.VendorId == userId));
                    await _context.SaveChangesAsync();

                    // ── 5. Orders (as buyer or vendor) ──
                    _context.RemoveRange(_context.Set<Order>().Where(o => orderIds.Contains(o.Id)));
                    await _context.SaveChangesAsync();

                    // ── 6. Listings owned by this user + their images/saved copies ──
                    var listingIds = await _context.Set<Listing>().Where(l => l.OwnerId == userId).Select(l => l.Id).ToListAsync();
                    _context.RemoveRange(_context.Set<ListingImage>().Where(li => listingIds.Contains(li.ListingId)));
                    _context.RemoveRange(_context.Set<SavedListing>().Where(sl => listingIds.Contains(sl.ListingId) || sl.UserId == userId));
                    await _context.SaveChangesAsync();
                    _context.RemoveRange(_context.Set<Listing>().Where(l => listingIds.Contains(l.Id)));
                    await _context.SaveChangesAsync();

                    // ── 7. Supplier-side assets/operators/wallet, if this user is a supplier ──
                    if (supplierProfile != null)
                    {
                        var assetIds = await _context.Set<Asset>().Where(a => a.SupplierId == supplierProfile.Id).Select(a => a.Id).ToListAsync();
                        var operatorIds = await _context.Set<Operator>().Where(o => o.SupplierId == supplierProfile.Id).Select(o => o.Id).ToListAsync();

                        _context.RemoveRange(_context.Set<AssetOperator>().Where(ao => assetIds.Contains(ao.AssetId) || operatorIds.Contains(ao.OperatorId)));
                        _context.RemoveRange(_context.Set<Guarantor>().Where(g => operatorIds.Contains(g.OperatorId)));
                        await _context.SaveChangesAsync();

                        _context.RemoveRange(_context.Set<Asset>().Where(a => assetIds.Contains(a.Id)));
                        _context.RemoveRange(_context.Set<Operator>().Where(o => operatorIds.Contains(o.Id)));
                        await _context.SaveChangesAsync();

                        var wallet = await _context.Set<SupplierWallet>().FirstOrDefaultAsync(w => w.SupplierId == supplierProfile.Id);
                        if (wallet != null)
                        {
                            _context.RemoveRange(_context.Set<WalletTransaction>().Where(t => t.WalletId == wallet.Id));
                            await _context.SaveChangesAsync();
                            _context.Remove(wallet);
                        }

                        await _context.SaveChangesAsync();
                        _context.Remove(supplierProfile);
                    }

                    // ── 8. Everything else keyed directly by UserId ──
                    _context.RemoveRange(_context.Set<Payout>().Where(p => p.VendorId == userId));
                    _context.RemoveRange(_context.Set<Revenue>().Where(r => r.VendorId == userId));
                    _context.RemoveRange(_context.Set<SubscriptionInvoice>().Where(si => si.UserId == userId));
                    _context.RemoveRange(_context.Set<Subscription>().Where(s => s.UserId == userId));
                    _context.RemoveRange(_context.Set<Notification>().Where(n => n.UserId == userId));
                    _context.RemoveRange(_context.Set<OtpEntry>().Where(o => o.UserId == userId));

                    var vendorProfile = await _context.Set<VendorProfile>().FirstOrDefaultAsync(v => v.UserId == userId);
                    if (vendorProfile != null) _context.Remove(vendorProfile);

                    await _context.SaveChangesAsync();

                    // ── 9. The user account itself ──
                    var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == userId);
                    if (user != null) _context.Remove(user);
                    await _context.SaveChangesAsync();

                    await tx.CommitAsync();
                    return true;
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            });
        }
    }
}