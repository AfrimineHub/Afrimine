using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class ListingRepository : RepositoryBase<Listing>, IListingRepository
    {
        public ListingRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Listing>> GetRecommendedAsync(string userId, int count = 6) =>
            await FindByCondition(x => !x.IsDeleted && x.OwnerId != userId, false)
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();

        public async Task<Listing?> GetByIdAsync(Guid id) =>
            await FindByCondition(x => x.Id == id && !x.IsDeleted, false)
                .FirstOrDefaultAsync();
    }
}
