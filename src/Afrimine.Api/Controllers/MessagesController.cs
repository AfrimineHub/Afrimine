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

        /// <summary>Get conversation list (sidebar)</summary>
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
        [HttpPost("conversations")]
        [ProducesResponseType(typeof(ApiResponse<ConversationListItemDto>), 201)]
        public async Task<IActionResult> StartConversation([FromBody] StartConversationDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Messaging.StartConversationAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get message thread for a conversation</summary>
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
        [HttpPost("conversations/{id:guid}/messages")]
        [ProducesResponseType(typeof(ApiResponse<MessageDto>), 201)]
        public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Messaging.SendMessageAsync(userId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get linked listing/order context for a conversation</summary>
        [HttpGet("conversations/{id:guid}/context")]
        [ProducesResponseType(typeof(ApiResponse<ConversationContextDto>), 200)]
        public async Task<IActionResult> GetContext(Guid id)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Messaging.GetContextAsync(userId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Mark conversation as read — drives unreadMessagesCount</summary>
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
