using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Shared.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Afrimine.Api.Controllers
{
    [Route("api/v{version:apiversion}/dashboard")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = Roles.AllUsers)]
    public class DashboardController : ControllerBase
    {
        private readonly IServiceManager _service;

        public DashboardController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>Get dashboard summary — saved listings count, unread messages, ongoing orders</summary>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(ApiResponse<DashboardSummaryDto>), 200)]
        public async Task<IActionResult> GetSummary()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.GetSummaryAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get recommended listings for the logged-in user</summary>
        [HttpGet("recommended")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ListingCardDto>>), 200)]
        public async Task<IActionResult> GetRecommended()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.GetRecommendedListingsAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get latest notifications</summary>
        [HttpGet("notifications")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationDto>>), 200)]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.GetLatestNotificationsAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Mark all notifications as read</summary>
        [HttpPatch("notifications/read")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> MarkNotificationsRead()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.MarkNotificationsReadAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get saved listings (paginated)</summary>
        [HttpGet("saved-listings")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SavedListingDto>>), 200)]
        public async Task<IActionResult> GetSavedListings([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.GetSavedListingsAsync(userId, page, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Save a listing</summary>
        [HttpPost("saved-listings")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SaveListing([FromBody] SaveListingRequestDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.SaveListingAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Remove a listing from saved</summary>
        [HttpDelete("saved-listings/{listingId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> UnsaveListing(Guid listingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.UnsaveListingAsync(userId, listingId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
