using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afrimine.Repository
{
    public class PayoutRepository : RepositoryBase<Payout>, IPayoutRepository
    {
        public PayoutRepository(AppDbContext context) : base(context) { }

        public new async Task Create(Payout entity) => await base.Create(entity);
        public new void Update(Payout entity) => base.Update(entity);

        public async Task<decimal> GetPendingAmountAsync(string vendorId) =>
            await FindByCondition(x => x.VendorId == vendorId
                && x.Status == PayoutStatus.Pending && !x.IsDeleted, false)
                .SumAsync(x => x.Amount);

        public async Task<decimal> GetTotalPaidAsync(string vendorId) =>
            await FindByCondition(x => x.VendorId == vendorId
                && x.Status == PayoutStatus.Completed && !x.IsDeleted, false)
                .SumAsync(x => x.Amount);

        public async Task<IEnumerable<Payout>> GetRecentAsync(string vendorId, int count = 5) =>
            await FindByCondition(x => x.VendorId == vendorId && !x.IsDeleted, false)
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();
    }
}
