using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Repository
{
    public interface IEquipmentRepository
    {
        // Supplier
        Task CreateSupplierProfileAsync(SupplierProfile profile);
        Task<SupplierProfile?> GetSupplierProfileAsync(Guid supplierId);
        void UpdateSupplierProfile(SupplierProfile profile);

        // Assets
        Task CreateAssetAsync(Asset asset);
        Task<Asset?> GetAssetAsync(Guid assetId);
        Task<IEnumerable<Asset>> GetAssetsBySupplierAsync(Guid supplierId);
        void UpdateAsset(Asset asset);
        Task<int> CountAssetsAsync(Guid supplierId);

        // Operators
        Task CreateOperatorAsync(Operator op);
        Task<Operator?> GetOperatorAsync(Guid operatorId);
        Task<IEnumerable<Operator>> GetOperatorsBySupplierAsync(Guid supplierId);
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
        Task<int> CountActiveBookingsAsync(Guid supplierId);

        // Daily checks & disputes
        Task AddDailyCheckAsync(DailyCheck check);
        Task AddDisputeAsync(BookingDispute dispute);
        Task<IEnumerable<BookingDispute>> GetBookingDisputesAsync(Guid bookingId);
        Task<IEnumerable<BookingDispute>> GetSupplierDisputesAsync(Guid supplierId);

        // Wallet
        Task CreateWalletAsync(SupplierWallet wallet);
        Task<SupplierWallet?> GetWalletAsync(Guid supplierId);
        void UpdateWallet(SupplierWallet wallet);
        Task AddWalletTransactionAsync(WalletTransaction transaction);
        Task<IEnumerable<WalletTransaction>> GetWalletTransactionsAsync(Guid supplierId);
        Task<(IEnumerable<Asset> Items, int TotalCount)> SearchAssetsAsync(string? q, MachineType? machineType, string? location, decimal? maxDailyRate, bool availableOnly,int page, int pageSize);
        Task<SupplierProfile?> GetSupplierProfileByUserIdAsync(string userId);

        void DeleteSupplierProfile(SupplierProfile profile);
        void DeleteWallet(SupplierWallet wallet);
        void DeleteAsset(Asset asset);
        // Admin
        Task<(IEnumerable<Booking> Items, int TotalCount)> GetAllBookingsAdminAsync(string? q, BookingStatus? status, int page, int pageSize);
        Task<Booking?> GetBookingByIdAdminAsync(Guid bookingId);
    }
}
