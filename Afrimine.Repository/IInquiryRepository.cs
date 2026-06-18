using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IInquiryRepository : IRepositoryBase<Inquiry>
    {
        Task<bool> ExistsAsync(string buyerId, Guid listingId);
    }
}
