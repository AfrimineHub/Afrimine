using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Repository
{
    public interface IQuoteRepository : IRepositoryBase<Quote>
    {
        Task<(IEnumerable<Quote> Items, int TotalCount)> GetVendorQuotesAsync(
            string vendorId, int page, int pageSize, QuoteStatus? status);
        Task<int> CountActiveAsync(string vendorId);
    }
}
