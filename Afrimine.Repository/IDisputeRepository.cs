using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Repository
{
    public interface IDisputeRepository : IRepositoryBase<Dispute>
    {
        Task<(IEnumerable<Dispute> Items, int TotalCount)> GetAllAsync(int page, int pageSize, DisputeStatus? status);
        Task<Dispute?> GetByIdAsync(Guid id);
        Task<Dispute?> GetByOrderIdAsync(Guid orderId);
    }
}
