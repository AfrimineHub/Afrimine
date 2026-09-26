using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class InquiryRepository : RepositoryBase<Inquiry>, IInquiryRepository
    {
        public InquiryRepository(AppDbContext context) : base(context) { }
        public new async Task Create(Inquiry entity) => await base.Create(entity);
        public new void Update(Inquiry entity) => base.Update(entity);

        public async Task<bool> ExistsAsync(string buyerId, Guid listingId) =>
            await FindByCondition(x => x.BuyerId == buyerId
                && x.ListingId == listingId && !x.IsDeleted, false).AnyAsync();
    }
}
