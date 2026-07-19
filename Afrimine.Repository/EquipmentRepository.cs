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

        public async Task<SupplierProfile?> GetSupplierProfileAsync(string userId) =>
            await _context.Set<SupplierProfile>()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

        public void UpdateSupplierProfile(SupplierProfile profile) =>
            _context.Set<SupplierProfile>().Update(profile);

        public async Task CreateAssetAsync(Asset asset) =>
            await _context.Set<Asset>().AddAsync(asset);

        public async Task<Asset?> GetAssetAsync(Guid assetId) =>
            await _context.Set<Asset>()
                .Include(x => x.Operators).ThenInclude(ao => ao.Operator)
                .FirstOrDefaultAsync(x => x.Id == assetId && !x.IsDeleted);

        public async Task<IEnumerable<Asset>> GetAssetsBySupplierAsync(string supplierId) =>
            await _context.Set<Asset>()
                .Where(x => x.SupplierId == supplierId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public void UpdateAsset(Asset asset) => _context.Set<Asset>().Update(asset);

        public async Task<int> CountAssetsAsync(string supplierId) =>
            await _context.Set<Asset>().CountAsync(x => x.SupplierId == supplierId && !x.IsDeleted);

        public async Task CreateOperatorAsync(Operator op) =>
            await _context.Set<Operator>().AddAsync(op);

        public async Task<Operator?> GetOperatorAsync(Guid operatorId) =>
            await _context.Set<Operator>()
                .Include(x => x.Guarantors)
                .FirstOrDefaultAsync(x => x.Id == operatorId && !x.IsDeleted);

        public async Task<IEnumerable<Operator>> GetOperatorsBySupplierAsync(string supplierId) =>
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
            IQueryable<Booking> query = _context.Set<Booking>()
                .Include(x => x.Asset)
                .Include(x => x.Miner)
                .Include(x => x.Supplier)
                .Where(x => (x.MinerId == userId || x.SupplierId == userId) && !x.IsDeleted);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<Booking?> GetBookingByPayscrowRefAsync(string reference) =>
            await _context.Set<Booking>()
                .FirstOrDefaultAsync(x => x.PayscrowTransactionReference == reference);

        public void UpdateBooking(Booking booking) => _context.Set<Booking>().Update(booking);

        public async Task<int> CountActiveBookingsAsync(string supplierId) =>
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

        public async Task<IEnumerable<BookingDispute>> GetSupplierDisputesAsync(string supplierId) =>
            await _context.Set<BookingDispute>()
                .Include(x => x.Booking).ThenInclude(b => b.Asset)
                .Include(x => x.RaisedBy)
                .Where(x => x.Booking.SupplierId == supplierId)
                .OrderByDescending(x => x.CreatedAt).ToListAsync();

        public async Task CreateWalletAsync(SupplierWallet wallet) =>
            await _context.Set<SupplierWallet>().AddAsync(wallet);

        public async Task<SupplierWallet?> GetWalletAsync(string supplierId) =>
            await _context.Set<SupplierWallet>()
                .FirstOrDefaultAsync(x => x.SupplierId == supplierId);

        public void UpdateWallet(SupplierWallet wallet) =>
            _context.Set<SupplierWallet>().Update(wallet);

        public async Task AddWalletTransactionAsync(WalletTransaction transaction) =>
            await _context.Set<WalletTransaction>().AddAsync(transaction);

        public async Task<IEnumerable<WalletTransaction>> GetWalletTransactionsAsync(string supplierId) =>
            await _context.Set<WalletTransaction>()
                .Include(x => x.Wallet)
                .Where(x => x.Wallet.SupplierId == supplierId)
                .OrderByDescending(x => x.CreatedAt).ToListAsync();
    }
}
