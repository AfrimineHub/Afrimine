using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class QuoteRepository : RepositoryBase<Quote>, IQuoteRepository
    {
        public QuoteRepository(AppDbContext context) : base(context) { }

        public new async Task Create(Quote entity) => await base.Create(entity);
        public new void Update(Quote entity) => base.Update(entity);

        public async Task<(IEnumerable<Quote> Items, int TotalCount)> GetVendorQuotesAsync(string vendorId, int page, int pageSize, QuoteStatus? status)
        {
            IQueryable<Quote> query = FindByCondition(x => x.VendorId == vendorId && !x.IsDeleted, false)
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

        public async Task<int> CountActiveAsync(string vendorId) =>
            await FindByCondition(x => x.VendorId == vendorId
                && x.Status == QuoteStatus.Active && !x.IsDeleted, false)
                .CountAsync();
    }
}
