using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface ISubscriptionPlanRepository : IRepositoryBase<SubscriptionPlan>
    {
        Task<IEnumerable<SubscriptionPlan>> GetAllPlansAsync();
        Task<SubscriptionPlan?> GetByKeyAsync(string planKey);
    }
}
