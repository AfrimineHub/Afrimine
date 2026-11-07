using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class SavedListingRepository : RepositoryBase<SavedListing>, ISavedListingRepository
    {
        public SavedListingRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<SavedListing>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 10) =>
            await FindByCondition(x => x.UserId == userId && !x.IsDeleted, false)
                .Include(x => x.Listing)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByUserIdAsync(string userId) =>
            await FindByCondition(x => x.UserId == userId && !x.IsDeleted, false)
                .CountAsync();

        public async Task<SavedListing?> GetByUserAndListingAsync(string userId, Guid listingId) =>
            await FindByCondition(x => x.UserId == userId && x.ListingId == listingId, true)
                .FirstOrDefaultAsync();
    }
}
