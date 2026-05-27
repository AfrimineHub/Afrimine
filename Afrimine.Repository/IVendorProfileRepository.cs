using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IVendorProfileRepository
    {
        Task Create(VendorProfile profile);
        Task<VendorProfile?> GetByUserId(string userId);
    }
}
