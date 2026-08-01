using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly AppDbContext _context;

        public EquipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateSupplierProfileAsync(SupplierProfile profile) =>
            await _context.Set<SupplierProfile>().AddAsync(profile);

        public async Task<SupplierProfile?> GetSupplierProfileAsync(Guid supplierId) =>
            await _context.Set<SupplierProfile>()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == supplierId);

        public void UpdateSupplierProfile(SupplierProfile profile) =>
            _context.Set<SupplierProfile>().Update(profile);

        public async Task CreateAssetAsync(Asset asset) =>
            await _context.Set<Asset>().AddAsync(asset);

        public async Task<Asset?> GetAssetAsync(Guid assetId) =>
            await _context.Set<Asset>()
                .Include(x => x.Operators).ThenInclude(ao => ao.Operator)
                .FirstOrDefaultAsync(x => x.Id == assetId && !x.IsDeleted);

        public async Task<IEnumerable<Asset>> GetAssetsBySupplierAsync(Guid supplierId) =>
            await _context.Set<Asset>()
                .Where(x => x.SupplierId == supplierId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        public async Task<SupplierProfile?> GetSupplierProfileByUserIdAsync(string userId)
            => await _context.Set<SupplierProfile>()
            .FirstOrDefaultAsync(s => s.UserId == userId);

        public void UpdateAsset(Asset asset) => _context.Set<Asset>().Update(asset);

        public async Task<int> CountAssetsAsync(Guid supplierId) =>
            await _context.Set<Asset>().CountAsync(x => x.SupplierId == supplierId && !x.IsDeleted);

        public async Task CreateOperatorAsync(Operator op) =>
            await _context.Set<Operator>().AddAsync(op);

        public async Task<Operator?> GetOperatorAsync(Guid operatorId) =>
            await _context.Set<Operator>()
                .Include(x => x.Guarantors)
                .FirstOrDefaultAsync(x => x.Id == operatorId && !x.IsDeleted);

        public async Task<IEnumerable<Operator>> GetOperatorsBySupplierAsync(Guid supplierId) =>
            await _context.Set<Operator>()
                .Include(x => x.Guarantors)
                .Where(x => x.SupplierId == supplierId && !x.IsDeleted)
                .ToListAsync();

        public void UpdateOperator(Operator op) => _context.Set<Operator>().Update(op);

        public async Task AssignOperatorAsync(AssetOperator assetOperator) =>
            await _context.Set<AssetOperator>().AddAsync(assetOperator);

        public async Task AddGuarantorAsync(Guarantor guarantor) =>
            await _context.Set<Guarantor>().AddAsync(guarantor);

        public async Task<int> CountGuarantorsAsync(Guid operatorId) =>
            await _context.Set<Guarantor>().CountAsync(x => x.OperatorId == operatorId);

        public async Task CreateBookingAsync(Booking booking) =>
            await _context.Set<Booking>().AddAsync(booking);

        public async Task<Booking?> GetBookingDetailAsync(Guid bookingId) =>
            await _context.Set<Booking>()
                .Include(x => x.Asset)
                .Include(x => x.Miner)
                .Include(x => x.Supplier)
                .Include(x => x.Disputes)
                .FirstOrDefaultAsync(x => x.Id == bookingId && !x.IsDeleted);

        public async Task<IEnumerable<Booking>> GetBookingsAsync(string userId, BookingStatus? status)
        {
            var supplierId = Guid.Parse(userId);

            IQueryable<Booking> query = _context.Set<Booking>()
                .Include(x => x.Asset)
                .Include(x => x.Miner)
                .Include(x => x.Supplier)
                .Where(x => (x.MinerId == userId || x.SupplierId == supplierId) && !x.IsDeleted);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<Booking?> GetBookingByPayscrowRefAsync(string reference) =>
            await _context.Set<Booking>()
                .FirstOrDefaultAsync(x => x.PayscrowTransactionReference == reference);

        public void UpdateBooking(Booking booking) => _context.Set<Booking>().Update(booking);

        public async Task<int> CountActiveBookingsAsync(Guid supplierId) =>
            await _context.Set<Booking>()
                .CountAsync(x => x.SupplierId == supplierId
                    && x.Status == BookingStatus.Active && !x.IsDeleted);

        public async Task AddDailyCheckAsync(DailyCheck check) =>
            await _context.Set<DailyCheck>().AddAsync(check);

        public async Task AddDisputeAsync(BookingDispute dispute) =>
            await _context.Set<BookingDispute>().AddAsync(dispute);

        public async Task<IEnumerable<BookingDispute>> GetBookingDisputesAsync(Guid bookingId) =>
            await _context.Set<BookingDispute>()
                .Include(x => x.RaisedBy)
                .Where(x => x.BookingId == bookingId)
                .OrderByDescending(x => x.CreatedAt).ToListAsync();

        public async Task<IEnumerable<BookingDispute>> GetSupplierDisputesAsync(Guid supplierId) =>
            await _context.Set<BookingDispute>()
                .Include(x => x.Booking).ThenInclude(b => b.Asset)
                .Include(x => x.RaisedBy)
                .Where(x => x.Booking.SupplierId == supplierId)
                .OrderByDescending(x => x.CreatedAt).ToListAsync();

        public async Task CreateWalletAsync(SupplierWallet wallet) =>
            await _context.Set<SupplierWallet>().AddAsync(wallet);

        public async Task<SupplierWallet?> GetWalletAsync(Guid supplierId) =>
            await _context.Set<SupplierWallet>()
                .FirstOrDefaultAsync(x => x.SupplierId == supplierId);

        public void UpdateWallet(SupplierWallet wallet) =>
            _context.Set<SupplierWallet>().Update(wallet);

        public async Task AddWalletTransactionAsync(WalletTransaction transaction) =>
            await _context.Set<WalletTransaction>().AddAsync(transaction);

        public async Task<IEnumerable<WalletTransaction>> GetWalletTransactionsAsync(Guid supplierId) =>
            await _context.Set<WalletTransaction>()
                .Include(x => x.Wallet)
                .Where(x => x.Wallet.SupplierId == supplierId)
                .OrderByDescending(x => x.CreatedAt).ToListAsync();

        public async Task<(IEnumerable<Asset> Items, int TotalCount)> SearchAssetsAsync(string? q, MachineType? machineType, string? location,decimal? maxDailyRate, bool availableOnly,int page, int pageSize)
        {
            IQueryable<Asset> query = _context.Set<Asset>()
                .Include(x => x.Supplier)
                //.Include(x => x.Supp)
                .Where(x => !x.IsDeleted);

            if (availableOnly)
                query = query.Where(x => x.Status == AssetStatus.Available);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Brand.Contains(q)
                    || x.Model.Contains(q)
                    || x.Description!.Contains(q));

            if (machineType.HasValue)
                query = query.Where(x => x.MachineType == machineType.Value);

            if (maxDailyRate.HasValue)
                query = query.Where(x => x.DailyRentalRate <= maxDailyRate.Value);

            // Filter by location against supplier profile city
            if (!string.IsNullOrWhiteSpace(location))
            {
                var supplierIds = await _context.Set<SupplierProfile>()
                    .Where(p => p.PrimaryBaseCity!.Contains(location)
                             || p.YardAddress!.Contains(location))
                    .Select(p => p.Id)
                    .ToListAsync();

                query = query.Where(x => supplierIds.Contains(x.SupplierId));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
