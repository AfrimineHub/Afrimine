using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface IConversationRepository : IRepositoryBase<Conversation>
    {
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId);
        Task<Conversation?> GetByIdWithMessagesAsync(Guid id);
        Task<Conversation?> GetExistingAsync(string buyerId, string vendorId, Guid? listingId);
        Task<int> CountUnreadAsync(string userId);
        Task MarkConversationReadAsync(Guid conversationId, string userId);
    }
}
