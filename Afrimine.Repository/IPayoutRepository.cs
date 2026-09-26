using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IPayoutRepository : IRepositoryBase<Payout>
    {
        Task<decimal> GetPendingAmountAsync(string vendorId);
        Task<decimal> GetTotalPaidAsync(string vendorId);
        Task<IEnumerable<Payout>> GetRecentAsync(string vendorId, int count = 5);
    }
}
