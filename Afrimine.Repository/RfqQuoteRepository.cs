using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class RfqQuoteRepository : RepositoryBase<RfqQuote>, IRfqQuoteRepository
    {
        public RfqQuoteRepository(AppDbContext context) : base(context) { }
        public new async Task Create(RfqQuote entity) => await base.Create(entity);
        public new void Update(RfqQuote entity) => base.Update(entity);

        public async Task<IEnumerable<RfqQuote>> GetByRfqIdAsync(Guid rfqId) =>
            await FindByCondition(x => x.RfqId == rfqId && !x.IsDeleted, false)
                .Include(x => x.Vendor).ToListAsync();

        public async Task<RfqQuote?> GetByIdAsync(Guid id) =>
            await FindByCondition(x => x.Id == id && !x.IsDeleted, true)
                .Include(x => x.Vendor).FirstOrDefaultAsync();
        public async Task<int> CountByRfqIdAsync(Guid rfqId) =>
    await FindByCondition(x => x.RfqId == rfqId && !x.IsDeleted, false).CountAsync();
    }
}
