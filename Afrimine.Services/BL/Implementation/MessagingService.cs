using Afrimine.Model.Entities;
using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Implementation
{
    public class MessagingService : IMessagingService
    {
        private readonly IRepositoryManager _repository;

        public MessagingService(IRepositoryManager repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<ConversationListItemDto>>> GetConversationsAsync(string userId)
        {
            var conversations = await _repository.Conversation.GetUserConversationsAsync(userId);
            var result = conversations.Select(c =>
            {
                var isVendor = c.VendorId == userId;
                var participant = isVendor ? c.Buyer : c.Vendor;
                var lastMsg = c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
                var unread = c.Messages.Count(m => m.SenderId != userId && !m.IsRead);

                return new ConversationListItemDto
                {
                    Id = c.Id,
                    ParticipantId = participant.Id,
                    ParticipantName = participant.UserName ?? string.Empty,
                    LastMessage = lastMsg?.Content,
                    LastMessageAt = lastMsg?.CreatedAt,
                    UnreadCount = unread,
                    ListingId = c.ListingId,
                    ListingTitle = c.Listing?.Title,
                    RfqId = c.RfqId,
                    RfqTitle = c.Rfq?.Title
                };
            });

            return ApiResponse<IEnumerable<ConversationListItemDto>>.Ok(result);
        }

        public async Task<ApiResponse<ConversationListItemDto>> StartConversationAsync(string initiatorId, StartConversationDto request)
        {
            var isBuyerInitiating = !string.IsNullOrWhiteSpace(request.VendorId);

            var buyerId = isBuyerInitiating ? initiatorId : request.BuyerId;
            var vendorId = isBuyerInitiating ? request.VendorId : initiatorId;

            if (string.IsNullOrWhiteSpace(buyerId) || string.IsNullOrWhiteSpace(vendorId))
                return ApiResponse<ConversationListItemDto>.Fail("Recipient must be specified.", 400);

            if (buyerId == vendorId) 
                return ApiResponse<ConversationListItemDto>.Fail("Cannot start a conversation with yourself.", 400);

            Conversation conv;

            // Reuse existing thread only when tied to same listing or same RFQ
            // Fresh conversation (no listing, no rfq) always creates new
            if (request.ListingId.HasValue || request.RfqId.HasValue)
            {
                var existing = await _repository.Conversation.GetExistingAsync(
                    buyerId, vendorId, request.ListingId, request.RfqId);

                conv = existing ?? await CreateConversationAsync(buyerId, vendorId, request);
            }
            else
            {
                // No listing, no RFQ — always new conversation
                conv = await CreateConversationAsync(buyerId, vendorId, request);
            }

            await SendMessageAsync(initiatorId, conv.Id, new SendMessageDto
            {
                Content = request.InitialMessage
            });

            var participantId = isBuyerInitiating ? vendorId! : buyerId!;

            return ApiResponse<ConversationListItemDto>.Ok(new ConversationListItemDto
            {
                Id = conv.Id,
                ParticipantId = participantId,
                ParticipantName = string.Empty,
                LastMessage = request.InitialMessage,
                LastMessageAt = DateTime.UtcNow,
                UnreadCount = 0,
                ListingId = request.ListingId,
                RfqId = request.RfqId
            });
        }

        private async Task<Conversation> CreateConversationAsync(string buyerId, string vendorId, StartConversationDto request)
        {
            var conv = new Conversation
            {
                BuyerId = buyerId,
                VendorId = vendorId,
                ListingId = request.ListingId,
                RfqId = request.RfqId,
                OrderId = request.OrderId
            };
            await _repository.Conversation.Create(conv);
            await _repository.SaveAsync();
            return conv;
        }
        public async Task<ApiResponse<IEnumerable<MessageDto>>> GetMessagesAsync(string userId, Guid conversationId)
        {
            var conv = await _repository.Conversation.GetByIdWithMessagesAsync(conversationId);
            if (conv is null) return ApiResponse<IEnumerable<MessageDto>>.Fail("Conversation not found.", 404);
            if (conv.BuyerId != userId && conv.VendorId != userId)
                return ApiResponse<IEnumerable<MessageDto>>.Fail("Access denied.", 403);

            return ApiResponse<IEnumerable<MessageDto>>.Ok(conv.Messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender?.UserName ?? string.Empty,
                Content = m.Content,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt
            }));
        }

        public async Task<ApiResponse<MessageDto>> SendMessageAsync(string userId, Guid conversationId, SendMessageDto request)
        {
            var conv = await _repository.Conversation.GetByIdWithMessagesAsync(conversationId);
            if (conv is null) return ApiResponse<MessageDto>.Fail("Conversation not found.", 404);
            if (conv.BuyerId != userId && conv.VendorId != userId)
                return ApiResponse<MessageDto>.Fail("Access denied.", 403);

            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = userId,
                Content = request.Content
            };

            await _repository.MessageRepo.Create(message);
            conv.UpdatedAt = DateTime.UtcNow;
            _repository.Conversation.Update(conv);
            await _repository.SaveAsync();

            return ApiResponse<MessageDto>.Ok(new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = string.Empty,
                Content = message.Content,
                IsRead = false,
                CreatedAt = message.CreatedAt
            });
        }

        public async Task<ApiResponse<ConversationContextDto>> GetContextAsync(string userId, Guid conversationId)
        {
            var conv = await _repository.Conversation.GetByIdWithMessagesAsync(conversationId);
            if (conv is null) return ApiResponse<ConversationContextDto>.Fail("Conversation not found.", 404);
            if (conv.BuyerId != userId && conv.VendorId != userId)
                return ApiResponse<ConversationContextDto>.Fail("Access denied.", 403);

            return ApiResponse<ConversationContextDto>.Ok(new ConversationContextDto
            {
                ListingId = conv.ListingId,
                Title = conv.Listing?.Title,
                Location = conv.Listing?.Location,
                PriceRange = conv.Listing?.PriceDescription,
                ImageUrl = conv.Listing?.Images.FirstOrDefault(x => x.IsPrimary && !x.IsDeleted)?.ImageUrl
                           ?? conv.Listing?.Images.FirstOrDefault(x => !x.IsDeleted)?.ImageUrl,
                Category = conv.Listing?.CategoryType.ToString(),
                OrderId = conv.OrderId,
                OrderStatus = conv.Order?.Status.ToString()
            });
        }

        public async Task<ApiResponse<string>> MarkReadAsync(string userId, Guid conversationId)
        {
            var conv = await _repository.Conversation.GetByIdWithMessagesAsync(conversationId);
            if (conv is null) return ApiResponse<string>.Fail("Conversation not found.", 404);
            if (conv.BuyerId != userId && conv.VendorId != userId)
                return ApiResponse<string>.Fail("Access denied.", 403);

            await _repository.Conversation.MarkConversationReadAsync(conversationId, userId);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Marked as read.");
        }
    }
}
