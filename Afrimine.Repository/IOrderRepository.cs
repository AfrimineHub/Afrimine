using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Repository
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        Task<int> CountByUserAndStatusAsync(string userId, OrderStatus status);
        Task<int> CountSuccessfulOrdersAsync(string userId);
        Task<(decimal Amount, string Currency)> GetPendingPayoutAsync(string userId);
        Task<(IEnumerable<Order> Items, int TotalCount)> GetVendorOrdersAsync(string vendorId, int page, int pageSize, OrderStatus? status);
    }
}
