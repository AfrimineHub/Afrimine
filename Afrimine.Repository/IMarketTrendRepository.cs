using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IMarketTrendRepository : IRepositoryBase<MarketTrend>
    {
        Task<IEnumerable<MarketTrend>> GetLatestAsync(int count = 10);
    }
}
