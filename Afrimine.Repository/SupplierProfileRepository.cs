using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class SupplierProfileRepository : RepositoryBase<SupplierProfile>, ISupplierProfileRepository
    {
        public SupplierProfileRepository(AppDbContext context) : base(context) { }

        public async Task<SupplierProfile?> GetByUserId(string userId) =>
            await FindByCondition(x => x.UserId == userId, true)
                .FirstOrDefaultAsync();

        public new async Task Create(SupplierProfile profile) => await base.Create(profile);
        public new void Update(SupplierProfile profile) => base.Update(profile);

        public async Task<IEnumerable<SupplierProfile>> GetByIdsAsync(IEnumerable<Guid> ids) =>
            await FindByCondition(x => ids.Contains(x.Id), false)
                .Include(x => x.User).ToListAsync();
    }
}
