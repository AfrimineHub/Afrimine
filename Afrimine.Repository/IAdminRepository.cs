using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Repository
{
    public interface IAdminRepository
    {
        // Users
        Task<(IEnumerable<User> Items, int TotalCount)> GetUsersAsync(string? q, string? role, string? kycStatus, string? accountStatus, int page, int pageSize);
        Task<int> CountUsersAsync();
        Task<int> CountActiveUsersAsync();
        Task<int> CountKycVerifiedAsync();
        Task<int> CountVendorsAsync();
        // Quotes
        Task<(IEnumerable<Quote> Items, int TotalCount)> GetQuotesAsync(string? q, string? status, int page, int pageSize);

        // Orders
        Task<(IEnumerable<Order> Items, int TotalCount)> GetOrdersAsync(string? q, string? status, int page, int pageSize);
        Task<Order?> GetOrderDetailAsync(Guid orderId);
        Task<int> CountOrdersByStatusAsync(OrderStatus status);
        Task<int> CountAllOrdersAsync(string? q);

        // Revenue
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetTotalRevenueLastMonthAsync();
        Task<decimal> GetVendorPayoutsAsync();
        Task<decimal> GetVendorPayoutsLastMonthAsync();
        Task<decimal> GetPendingPaymentsAsync();
        Task<(IEnumerable<Revenue> Items, int TotalCount)> GetTransactionsAsync(string? status, int page, int pageSize);

        // Withdrawals
        Task<(IEnumerable<Payout> Items, int TotalCount)> GetWithdrawalsAsync(string? q, string? status, int page, int pageSize);
        Task<Payout?> GetPayoutByIdAsync(Guid payoutId);

        // KYC
        Task<(IEnumerable<SupplierProfile> Items, int TotalCount)> GetKycQueueAsync(string? q, string? status, int page, int pageSize);
        Task<SupplierProfile?> GetKycDetailAsync(Guid profileId);

        // Dashboard helpers
        Task<int> CountOpenDisputesAsync();
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);
        Task<IEnumerable<SupplierProfile>> GetRecentKycSubmissionsAsync(int count);
        Task<IEnumerable<Dispute>> GetOpenDisputesAsync(int count);
        // Listings
        Task<(IEnumerable<Listing> Items, int TotalCount)> GetListingsAsync(string? status, string? q, Guid? supplierId, int page, int pageSize);
        Task<int> CountListingsByStatusAsync(ListingStatus? status);
        Task<Dictionary<string, SupplierProfile>> GetSupplierProfilesByOwnerIdsAsync(IEnumerable<string> ownerIds);
        Task<(IEnumerable<Escrow> Items, int TotalCount)> GetEscrowPaymentsAsync(string? status, int page, int pageSize);
    }
}