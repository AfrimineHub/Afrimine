using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, int count = 10) =>
            await FindByCondition(x => x.UserId == userId && !x.IsDeleted, false)
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();

        public async Task<int> CountUnreadAsync(string userId) =>
            await FindByCondition(x => x.UserId == userId && !x.IsRead && !x.IsDeleted, false)
                .CountAsync();

        public async Task MarkAllAsReadAsync(string userId)
        {
            var unread = await FindByCondition(x => x.UserId == userId && !x.IsRead, true)
                .ToListAsync();

            foreach (var n in unread)
                n.IsRead = true;
        }
    }
}
