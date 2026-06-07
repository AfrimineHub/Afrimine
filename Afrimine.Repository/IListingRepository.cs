using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Repository
{
    public interface IListingRepository : IRepositoryBase<Listing>
    {
        Task<IEnumerable<Listing>> GetRecommendedAsync(string userId, int count = 6);
        Task<Listing?> GetByIdAsync(Guid id);

        Task<(IEnumerable<Listing> Items, int TotalCount)> GetVendorListingsAsync(
         string vendorId, int page, int pageSize,
         ListingStatus? status, string? search, ListingCategory? category);

        Task<Listing?> GetByIdWithImagesAsync(Guid id);
    }
}
