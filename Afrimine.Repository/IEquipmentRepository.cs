using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Repository
{
    public interface IEquipmentRepository
    {
        // Supplier
        Task CreateSupplierProfileAsync(SupplierProfile profile);
        Task<SupplierProfile?> GetSupplierProfileAsync(string userId);
        void UpdateSupplierProfile(SupplierProfile profile);

        // Assets
        Task CreateAssetAsync(Asset asset);
        Task<Asset?> GetAssetAsync(Guid assetId);
        Task<IEnumerable<Asset>> GetAssetsBySupplierAsync(string supplierId);
        void UpdateAsset(Asset asset);
        Task<int> CountAssetsAsync(string supplierId);

        // Operators
        Task CreateOperatorAsync(Operator op);
        Task<Operator?> GetOperatorAsync(Guid operatorId);
        Task<IEnumerable<Operator>> GetOperatorsBySupplierAsync(string supplierId);
        void UpdateOperator(Operator op);
        Task AssignOperatorAsync(AssetOperator assetOperator);
        Task AddGuarantorAsync(Guarantor guarantor);
        Task<int> CountGuarantorsAsync(Guid operatorId);

        // Bookings
        Task CreateBookingAsync(Booking booking);
        Task<Booking?> GetBookingDetailAsync(Guid bookingId);
        Task<IEnumerable<Booking>> GetBookingsAsync(string userId, BookingStatus? status);
        Task<Booking?> GetBookingByPayscrowRefAsync(string reference);
        void UpdateBooking(Booking booking);
        Task<int> CountActiveBookingsAsync(string supplierId);

        // Daily checks & disputes
        Task AddDailyCheckAsync(DailyCheck check);
        Task AddDisputeAsync(BookingDispute dispute);
        Task<IEnumerable<BookingDispute>> GetBookingDisputesAsync(Guid bookingId);
        Task<IEnumerable<BookingDispute>> GetSupplierDisputesAsync(string supplierId);

        // Wallet
        Task CreateWalletAsync(SupplierWallet wallet);
        Task<SupplierWallet?> GetWalletAsync(string supplierId);
        void UpdateWallet(SupplierWallet wallet);
        Task AddWalletTransactionAsync(WalletTransaction transaction);
        Task<IEnumerable<WalletTransaction>> GetWalletTransactionsAsync(string supplierId);
        Task<(IEnumerable<Asset> Items, int TotalCount)> SearchAssetsAsync(string? q, MachineType? machineType, string? location, decimal? maxDailyRate, bool availableOnly,int page, int pageSize);
    }
}
