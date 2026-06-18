using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Repository
{
    public interface IRfqRepository : IRepositoryBase<Rfq>
    {
        Task<(IEnumerable<Rfq> Items, int TotalCount)> GetBuyerRfqsAsync(string buyerId, int page, int pageSize);
        Task<(IEnumerable<Rfq> Items, int TotalCount)> GetOpenRfqsAsync(int page, int pageSize, RfqStatus? status);
        Task<Rfq?> GetByIdAsync(Guid id);
        Task<int> CountOpenByBuyerAsync(string buyerId);
    }
}
