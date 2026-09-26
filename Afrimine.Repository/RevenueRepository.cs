using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class RevenueRepository : RepositoryBase<Revenue>, IRevenueRepository
    {
        public RevenueRepository(AppDbContext context) : base(context) { }

        public new async Task Create(Revenue entity) => await base.Create(entity);
        public new void Update(Revenue entity) => base.Update(entity);

        public async Task<decimal> GetTotalInflowAsync(string vendorId) =>
            await FindByCondition(x => x.VendorId == vendorId && !x.IsDeleted, false)
                .SumAsync(x => x.Amount);

        public async Task<decimal> GetMonthInflowAsync(string vendorId, int year, int month) =>
            await FindByCondition(x => x.VendorId == vendorId && !x.IsDeleted
                && x.TransactionDate.Year == year
                && x.TransactionDate.Month == month, false)
                .SumAsync(x => x.Amount);
    }
}
