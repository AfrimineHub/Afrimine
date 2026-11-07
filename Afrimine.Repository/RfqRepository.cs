using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class RfqRepository : RepositoryBase<Rfq>, IRfqRepository
    {
        public RfqRepository(AppDbContext context) : base(context) { }

        public new async Task Create(Rfq entity) => await base.Create(entity);
        public new void Update(Rfq entity) => base.Update(entity);

        public async Task<(IEnumerable<Rfq> Items, int TotalCount)> GetBuyerRfqsAsync(string buyerId, int page, int pageSize)
        {
            IQueryable<Rfq> query = FindByCondition(x => x.BuyerId == buyerId && !x.IsDeleted, false)
                .Include(x => x.Buyer);
            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<(IEnumerable<Rfq> Items, int TotalCount)> GetOpenRfqsAsync(int page, int pageSize, RfqStatus? status)
        {
            IQueryable<Rfq> query = FindByCondition(x => !x.IsDeleted, false).Include(x => x.Buyer);
            query = status.HasValue
                ? query.Where(x => x.Status == status.Value)
                : query.Where(x => x.Status == RfqStatus.Open);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<Rfq?> GetByIdAsync(Guid id) =>
            await FindByCondition(x => x.Id == id && !x.IsDeleted, false)
                .Include(x => x.Buyer).FirstOrDefaultAsync();

        public async Task<int> CountOpenByBuyerAsync(string buyerId) =>
            await FindByCondition(x => x.BuyerId == buyerId
                && x.Status == RfqStatus.Open && !x.IsDeleted, false).CountAsync();
    }
}
