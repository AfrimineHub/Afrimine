using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class SubscriptionPlanRepository : RepositoryBase<SubscriptionPlan>, ISubscriptionPlanRepository
    {
        public SubscriptionPlanRepository(AppDbContext context) : base(context) { }
        public new async Task Create(SubscriptionPlan entity) => await base.Create(entity);
        public new void Update(SubscriptionPlan entity) => base.Update(entity);

        public async Task<IEnumerable<SubscriptionPlan>> GetAllPlansAsync() =>
            await FindByCondition(x => !x.IsDeleted, false)
                .OrderBy(x => x.PriceMonthly).ToListAsync();

        public async Task<SubscriptionPlan?> GetByKeyAsync(string planKey) =>
            await FindByCondition(x => x.PlanKey == planKey && !x.IsDeleted, false)
                .FirstOrDefaultAsync();
    }
}
