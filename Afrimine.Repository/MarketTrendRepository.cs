using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afrimine.Repository
{
    public class MarketTrendRepository : RepositoryBase<MarketTrend>, IMarketTrendRepository
    {
        public MarketTrendRepository(AppDbContext context) : base(context) { }
        public new async Task Create(MarketTrend entity) => await base.Create(entity);
        public new void Update(MarketTrend entity) => base.Update(entity);

        public async Task<IEnumerable<MarketTrend>> GetLatestAsync(int count = 10) =>
            await FindByCondition(x => !x.IsDeleted, false)
                .OrderByDescending(x => x.AsOf).Take(count).ToListAsync();
    }
}
