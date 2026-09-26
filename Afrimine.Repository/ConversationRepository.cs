using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class ConversationRepository : RepositoryBase<Conversation>, IConversationRepository
    {
        public ConversationRepository(AppDbContext context) : base(context) { }
        public new async Task Create(Conversation entity) => await base.Create(entity);
        public new void Update(Conversation entity) => base.Update(entity);

        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId) =>
            await FindByCondition(x => (x.BuyerId == userId || x.VendorId == userId) && !x.IsDeleted, false)
                .Include(x => x.Buyer)
                .Include(x => x.Vendor)
                .Include(x => x.Listing)
                .Include(x => x.Rfq)
                .Include(x => x.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();

        public async Task<Conversation?> GetByIdWithMessagesAsync(Guid id) =>
            await FindByCondition(x => x.Id == id && !x.IsDeleted, false)
                .Include(x => x.Buyer)
                .Include(x => x.Vendor)
                .Include(x => x.Listing).ThenInclude(l => l!.Images)
                .Include(x => x.Order)
                .Include(x => x.Messages.OrderBy(m => m.CreatedAt))
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync();

        public async Task<Conversation?> GetExistingAsync(string buyerId, string vendorId, Guid? listingId, Guid? rfqId) => 
            await FindByCondition(x => x.BuyerId == buyerId && x.VendorId == vendorId && 
            x.ListingId == listingId &&
            x.RfqId == rfqId &&
            !x.IsDeleted, true)
            .FirstOrDefaultAsync();

        public async Task<int> CountUnreadAsync(string userId) =>
            await AppDbContext.Set<Message>()
                .Where(m => !m.IsRead && m.SenderId != userId && !m.IsDeleted
                    && (m.Conversation.BuyerId == userId || m.Conversation.VendorId == userId))
                .CountAsync();

        public async Task MarkConversationReadAsync(Guid conversationId, string userId)
        {
            var messages = await AppDbContext.Set<Message>()
                .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead)
                .ToListAsync();
            foreach (var m in messages) m.IsRead = true;
        }
    }
}
