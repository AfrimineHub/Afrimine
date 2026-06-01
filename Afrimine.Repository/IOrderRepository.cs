using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Repository
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        Task<int> CountByUserAndStatusAsync(string userId, OrderStatus status);
    }
}
