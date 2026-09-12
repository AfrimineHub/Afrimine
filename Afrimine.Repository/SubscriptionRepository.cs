using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class SubscriptionRepository : RepositoryBase<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(AppDbContext context) : base(context) { }

        public new async Task Create(Subscription entity) => await base.Create(entity);
        public new void Update(Subscription entity) => base.Update(entity);

        public async Task<Subscription?> GetActiveByUserIdAsync(string userId) =>
            await FindByCondition(x => x.UserId == userId && x.IsActive && !x.IsDeleted, false)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
    }
}
