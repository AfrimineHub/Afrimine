using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IListingImageRepository : IRepositoryBase<ListingImage>
    {
        Task<ListingImage?> GetByIdAndListingAsync(Guid imageId, Guid listingId);
        Task<int> CountByListingAsync(Guid listingId);
    }
}
