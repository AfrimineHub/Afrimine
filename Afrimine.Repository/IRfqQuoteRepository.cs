using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IRfqQuoteRepository : IRepositoryBase<RfqQuote>
    {
        Task<IEnumerable<RfqQuote>> GetByRfqIdAsync(Guid rfqId);
        Task<RfqQuote?> GetByIdAsync(Guid id);
        Task<int> CountByRfqIdAsync(Guid rfqId);
    }
}
