using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface ISubscriptionInvoiceRepository : IRepositoryBase<SubscriptionInvoice>
    {
        Task<(IEnumerable<SubscriptionInvoice> Items, int TotalCount)> GetByUserAsync(string userId, int page, int pageSize);
        Task<SubscriptionInvoice?> GetBySessionIdAsync(string sessionId);
    }
}
