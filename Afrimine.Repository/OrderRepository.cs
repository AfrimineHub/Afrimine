using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }
        public new async Task Create(Order entity) => await base.Create(entity);
        public new void Update(Order entity) => base.Update(entity); 

        public async Task<int> CountByUserAndStatusAsync(string userId, OrderStatus status) =>
            await FindByCondition(x => x.BuyerId == userId && x.Status == status && !x.IsDeleted, false)
                .CountAsync();

        public async Task<int> CountSuccessfulOrdersAsync(string userId) =>
    await FindByCondition(x => x.BuyerId == userId && x.Status == OrderStatus.Completed && !x.IsDeleted, false)
        .CountAsync();

        public async Task<(decimal Amount, string Currency)> GetPendingPayoutAsync(string userId)
        {
            // TODO: replace with your actual payout/payment table query when built
            // For now returns zero until payout tracking is implemented
            return await Task.FromResult((0m, "USD"));
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetVendorOrdersAsync(string vendorId, int page, int pageSize, OrderStatus? status)
        {
            IQueryable<Order> query = FindByCondition(
                x => x.Listing.OwnerId == vendorId && !x.IsDeleted, false)
                .Include(x => x.Listing)
                .Include(x => x.Buyer);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetBuyerOrdersAsync(
            string buyerId, int page, int pageSize, OrderStatus? status)
        {
            IQueryable<Order> query = FindByCondition(x => x.BuyerId == buyerId && !x.IsDeleted, false)
                .Include(x => x.Listing).Include(x => x.Vendor);
            if (status.HasValue) query = query.Where(x => x.Status == status.Value);
            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<Order?> GetByIdAsync(Guid id) =>
            await FindByCondition(x => x.Id == id && !x.IsDeleted, true)
                .Include(x => x.Listing).Include(x => x.Buyer).Include(x => x.Vendor)
                .FirstOrDefaultAsync();
    }
}
