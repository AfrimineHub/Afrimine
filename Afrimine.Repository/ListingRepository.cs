using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class ListingRepository : RepositoryBase<Listing>, IListingRepository
    {
        public ListingRepository(AppDbContext context) : base(context) { }

        public async Task<(IEnumerable<Listing> Items, int TotalCount)> GetVendorListingsAsync(string vendorId, int page, int pageSize, ListingStatus? status, string? search, ListingCategory? category)
        {
            IQueryable<Listing> query = FindByCondition(x => x.OwnerId == vendorId && !x.IsDeleted, false)
                .Include(x => x.Images);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Title.Contains(search) || x.Description.Contains(search) || x.Location.Contains(search));

            if (category.HasValue)
                query = query.Where(x => x.CategoryType == category.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Listing?> GetByIdWithImagesAsync(Guid id) =>
            await FindByCondition(x => x.Id == id && !x.IsDeleted, false)
                .Include(x => x.Images)
                .FirstOrDefaultAsync();

        public async Task<Listing?> GetByIdAsync(Guid id) =>
            await FindByCondition(x => x.Id == id && !x.IsDeleted, false)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<Listing>> GetRecommendedAsync(string userId, int count = 6) =>
            await FindByCondition(x => !x.IsDeleted && x.OwnerId != userId && x.Status == ListingStatus.Active, false)
                .Include(x => x.Images)
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();
    }
}
