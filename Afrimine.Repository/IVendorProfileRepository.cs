using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IVendorProfileRepository
    {
        Task Create(VendorProfile profile);
        Task<VendorProfile?> GetByUserId(string userId);
        Task<IEnumerable<VendorProfile>> GetByUserIdsAsync(IEnumerable<Guid> userIds);
        void Update(VendorProfile profile);
    }
}
