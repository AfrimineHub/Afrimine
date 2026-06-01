using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }

        public async Task<int> CountByUserAndStatusAsync(string userId, OrderStatus status) =>
            await FindByCondition(x => x.BuyerId == userId && x.Status == status && !x.IsDeleted, false)
                .CountAsync();
    }
}
