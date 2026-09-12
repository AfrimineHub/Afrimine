using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface INotificationRepository : IRepositoryBase<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, int count = 10);
        Task<int> CountUnreadAsync(string userId);
        Task MarkAllAsReadAsync(string userId);
    }
}
