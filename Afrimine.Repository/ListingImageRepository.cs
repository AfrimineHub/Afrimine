using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class ListingImageRepository : RepositoryBase<ListingImage>, IListingImageRepository
    {
        public ListingImageRepository(AppDbContext context) : base(context) { }

        public async Task<ListingImage?> GetByIdAndListingAsync(Guid imageId, Guid listingId) =>
            await FindByCondition(x => x.Id == imageId && x.ListingId == listingId && !x.IsDeleted, true)
                .FirstOrDefaultAsync();

        public async Task<int> CountByListingAsync(Guid listingId) =>
            await FindByCondition(x => x.ListingId == listingId && !x.IsDeleted, false)
                .CountAsync();

        public new async Task Create(ListingImage entity) => await base.Create(entity);
        public new void Update(ListingImage entity) => base.Update(entity);
    }
}
