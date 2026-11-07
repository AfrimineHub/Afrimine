using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IMessagingService
    {
        Task<ApiResponse<IEnumerable<ConversationListItemDto>>> GetConversationsAsync(string userId);
        Task<ApiResponse<IEnumerable<MessageDto>>> GetMessagesAsync(string userId, Guid conversationId);
        Task<ApiResponse<MessageDto>> SendMessageAsync(string userId, Guid conversationId, SendMessageDto request);
        Task<ApiResponse<ConversationContextDto>> GetContextAsync(string userId, Guid conversationId);
        Task<ApiResponse<ConversationListItemDto>> StartConversationAsync(string buyerId, StartConversationDto request);
        Task<ApiResponse<string>> MarkReadAsync(string userId, Guid conversationId);
    }
}
