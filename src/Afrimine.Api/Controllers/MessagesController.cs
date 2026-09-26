using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Shared.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Afrimine.Api.Controllers
{
    [Route("api/v{version:apiversion}/messages")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = Roles.AllUsers)]
    public class MessagesController : ControllerBase
    {
        private readonly IServiceManager _service;

        public MessagesController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>Get all conversations (chat sidebar)</summary>
        /// <remarks>
        /// Returns all conversations for the authenticated user ordered by most recent.
        /// Each item shows last message preview and unread count.
        /// Conversations are created automatically when a listing inquiry is sent.
        /// </remarks>
        [HttpGet("conversations")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ConversationListItemDto>>), 200)]
        public async Task<IActionResult> GetConversations()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Messaging.GetConversationsAsync(userId);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Start a new conversation</summary>
        /// <remarks>
        /// Creates a new conversation thread. If a conversation already exists for the same buyer+vendor+listing, the existing thread is reused.
        ///
        /// **For Buyers:** Set `vendorId` — leave `buyerId` empty
        /// **For Vendors:** Set `buyerId` — leave `vendorId` empty (creates a fresh thread)
        ///
        /// **Rules:**
        /// - If `listingId` or `rfqId` is provided → reuses existing thread for that listing/RFQ
        /// - If neither is provided → always creates a new thread (vendor outreach)
        /// </remarks>
        [HttpPost("conversations")]
        [ProducesResponseType(typeof(ApiResponse<ConversationListItemDto>), 201)]
        public async Task<IActionResult> StartConversation([FromBody] StartConversationDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Messaging.StartConversationAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get all messages in a conversation (chat window)</summary>
        /// <remarks>
        /// Returns messages ordered chronologically (oldest first).
        /// Only participants (buyer or vendor) can access a conversation.
        /// </remarks>

        [HttpGet("conversations/{id:guid}/messages")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MessageDto>>), 200)]
        public async Task<IActionResult> GetMessages(Guid id)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Messaging.GetMessagesAsync(userId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Send a message in a conversation</summary>
        /// <remarks>Only participants can send messages in a conversation.</remarks> 
        [HttpPost("conversations/{id:guid}/messages")]
        [ProducesResponseType(typeof(ApiResponse<MessageDto>), 201)]
        public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Messaging.SendMessageAsync(userId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get linked listing or order context for a conversation</summary>
        /// <remarks>
        /// Returns the listing snippet or order info linked to this conversation.
        /// Used for the OrderContextCard / listing snippet shown in the chat sidebar.
        /// </remarks>
        [HttpGet("conversations/{id:guid}/context")]
        [ProducesResponseType(typeof(ApiResponse<ConversationContextDto>), 200)]
        public async Task<IActionResult> GetContext(Guid id)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Messaging.GetContextAsync(userId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Mark all messages in a conversation as read</summary>
        /// <remarks>Resets the unread count for this conversation. Call this when the user opens the chat window.</remarks>
        [HttpPatch("conversations/{id:guid}/read")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Messaging.MarkReadAsync(userId, id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
