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

        public async Task<int> CountByVendorAsync(string vendorId) => 
            await FindByCondition(x => x.OwnerId == vendorId && !x.IsDeleted, false)
            .CountAsync();

        public async Task<int> CountActiveQuotesAsync(string vendorId) =>
            await FindByCondition(x => x.OwnerId == vendorId && !x.IsDeleted
                && x.Status == ListingStatus.Active, false)
                .CountAsync();

        public async Task<IEnumerable<Listing>> GetPerformanceListingsAsync(string vendorId, int page, int pageSize) => 
            await FindByCondition(x => x.OwnerId == vendorId && !x.IsDeleted, false)
            .Include(x => x.SavedByUsers)
            .OrderByDescending(x => x.ViewsCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        public async Task<int> CountTotalAsync(string vendorId) =>
            await FindByCondition(x => x.OwnerId == vendorId && !x.IsDeleted, false)
                .CountAsync();

        public async Task<(IEnumerable<Listing> Items, int TotalCount)> SearchMarketplaceAsync(string? q, string? location, string? mineral, ListingCategory? listingType, bool verifiedOnly,int page, int pageSize)
        {
            IQueryable<Listing> query = FindByCondition(
                x => !x.IsDeleted && x.Status == ListingStatus.Active, false)
                .Include(x => x.Images)
                .Include(x => x.Owner);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Title.Contains(q) || x.Description.Contains(q) || x.Location.Contains(q));
            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(x => x.Location.Contains(location) || x.Country.Contains(location));
            if (!string.IsNullOrWhiteSpace(mineral))
                query = query.Where(x => x.MineralType != null && x.MineralType.Contains(mineral));
            if (listingType.HasValue)
                query = query.Where(x => x.CategoryType == listingType.Value);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync() =>
            await FindByCondition(x => !x.IsDeleted && x.Status == ListingStatus.Active, false)
                .Select(x => x.CategoryType.ToString()).Distinct().ToListAsync();
    }
}
