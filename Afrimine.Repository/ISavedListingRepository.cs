using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface ISavedListingRepository : IRepositoryBase<SavedListing>
    {
        Task<IEnumerable<SavedListing>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 10);
        Task<int> CountByUserIdAsync(string userId);
        Task<SavedListing?> GetByUserAndListingAsync(string userId, Guid listingId);
    }
}
