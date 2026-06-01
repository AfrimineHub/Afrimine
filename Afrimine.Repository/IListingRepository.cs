using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IListingRepository
    {
        Task<IEnumerable<Listing>> GetRecommendedAsync(string userId, int count = 6);
        Task<Listing?> GetByIdAsync(Guid id);
    }
}
