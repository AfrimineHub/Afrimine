using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class VendorProfileRepository : RepositoryBase<VendorProfile>, IVendorProfileRepository
    {
        public VendorProfileRepository(AppDbContext context) : base(context) { }

        public async Task<VendorProfile?> GetByUserId(string userId) =>
            await FindByCondition(x => x.UserId == userId, true)
                .FirstOrDefaultAsync();
        public new async Task Create(VendorProfile profile) => await base.Create(profile);
        public new void Update(VendorProfile profile) => base.Update(profile);

        public async Task<IEnumerable<VendorProfile>> GetByUserIdsAsync(IEnumerable<Guid> userIds) =>
            await FindByCondition(x => userIds.Contains(x.Id), false)
                .Include(x => x.User).ToListAsync();
    }
}
