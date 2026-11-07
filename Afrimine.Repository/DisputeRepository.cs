using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class DisputeRepository : RepositoryBase<Dispute>, IDisputeRepository
    {
        public DisputeRepository(AppDbContext context) : base(context) { }
        public new async Task Create(Dispute entity) => await base.Create(entity);
        public new void Update(Dispute entity) => base.Update(entity);

        public async Task<(IEnumerable<Dispute> Items, int TotalCount)> GetAllAsync(
            int page, int pageSize, DisputeStatus? status)
        {
            IQueryable<Dispute> query = FindByCondition(x => !x.IsDeleted, false)
                .Include(x => x.Order).ThenInclude(o => o.Listing)
                .Include(x => x.RaisedBy);
            if (status.HasValue) query = query.Where(x => x.Status == status.Value);
            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<Dispute?> GetByIdAsync(Guid id) =>
            await FindByCondition(x => x.Id == id && !x.IsDeleted, true)
                .Include(x => x.Order).ThenInclude(o => o.Listing)
                .Include(x => x.RaisedBy).FirstOrDefaultAsync();

        public async Task<Dispute?> GetByOrderIdAsync(Guid orderId) =>
            await FindByCondition(x => x.OrderId == orderId && !x.IsDeleted, true)
                .FirstOrDefaultAsync();
    }
}
