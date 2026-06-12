using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface ISubscriptionRepository : IRepositoryBase<Subscription>
    {
        Task<Subscription?> GetActiveByUserIdAsync(string userId);
    }
}
