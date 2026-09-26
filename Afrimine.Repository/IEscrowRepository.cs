using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IEscrowRepository : IRepositoryBase<Escrow>
    {
        Task<Escrow?> GetByOrderIdAsync(Guid orderId);
    }
}
