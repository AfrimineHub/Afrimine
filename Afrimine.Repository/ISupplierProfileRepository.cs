using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface ISupplierProfileRepository
    {
        Task<SupplierProfile?> GetByUserId(string userId);
        Task Create(SupplierProfile profile);
        void Update(SupplierProfile profile);
        Task<IEnumerable<SupplierProfile>> GetByIdsAsync(IEnumerable<Guid> ids);
    }
}
