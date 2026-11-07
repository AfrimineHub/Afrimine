using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class SubscriptionInvoiceRepository : RepositoryBase<SubscriptionInvoice>, ISubscriptionInvoiceRepository
    {
        public SubscriptionInvoiceRepository(AppDbContext context) : base(context) { }
        public new async Task Create(SubscriptionInvoice entity) => await base.Create(entity);
        public new void Update(SubscriptionInvoice entity) => base.Update(entity);

        public async Task<(IEnumerable<SubscriptionInvoice> Items, int TotalCount)> GetByUserAsync(string userId, int page, int pageSize)
        {
            IQueryable<SubscriptionInvoice> query = FindByCondition(x => x.UserId == userId && !x.IsDeleted, false)
                .OrderByDescending(x => x.CreatedAt);
            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<SubscriptionInvoice?> GetBySessionIdAsync(string sessionId) =>
            await FindByCondition(x => x.SessionId == sessionId, true).FirstOrDefaultAsync();
    }
}
