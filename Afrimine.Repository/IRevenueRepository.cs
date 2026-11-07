using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IRevenueRepository : IRepositoryBase<Revenue>
    {
        Task<decimal> GetTotalInflowAsync(string vendorId);
        Task<decimal> GetMonthInflowAsync(string vendorId, int year, int month);
    }
}
