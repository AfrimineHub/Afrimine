using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class InvestmentInsightRepository : RepositoryBase<InvestmentInsight>, IInvestmentInsightRepository
    {
        public InvestmentInsightRepository(AppDbContext context) : base(context) { }
        public new async Task Create(InvestmentInsight entity) => await base.Create(entity);
        public new void Update(InvestmentInsight entity) => base.Update(entity);

        public async Task<IEnumerable<InvestmentInsight>> GetLatestAsync(int count = 10) =>
            await FindByCondition(x => !x.IsDeleted, false)
                .OrderByDescending(x => x.PublishedAt).Take(count).ToListAsync();
    }
}
