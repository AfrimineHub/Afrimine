using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Shared.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Afrimine.Api.Controllers.V1
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

        /// <summary>Get dashboard summary stats</summary>
        /// <remarks>
        /// Returns counts for the top stats bar on the dashboard.
        /// Works for both Vendors and Buyers — returns relevant counts per role.
        ///
        /// **Response includes:**
        /// - `savedListingsCount` — bookmarked listings
        /// - `unreadMessagesCount` — unread conversation messages
        /// - `ongoingOrdersCount` — active orders/bookings
        /// - `openRfqsCount` — open RFQs (buyers only)
        /// - `totalListingsCount` — total listings published (vendors only)
        /// - `pendingPayoutAmount` — funds pending withdrawal (vendors only)
        /// </remarks>
        /// 
        [HttpGet("summary")]
        [ProducesResponseType(typeof(ApiResponse<DashboardSummaryDto>), 200)]
        public async Task<IActionResult> GetSummary()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.GetSummaryAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get personalized listing recommendations</summary>
        /// <remarks>
        /// Returns up to 6 active listings from other vendors, ordered by most recent.
        /// Used for the "Recommended For You" section on the dashboard.
        /// </remarks>
        [HttpGet("recommended")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ListingCardDto>>), 200)]
        public async Task<IActionResult> GetRecommended()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.GetRecommendedListingsAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get latest notifications for the authenticated user</summary>
        /// <remarks>Returns the 10 most recent notifications ordered by date descending.</remarks>
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
        /// <remarks>Sets all unread notifications to read. This drives the unread count back to 0.</remarks>
        [HttpPatch("notifications/read")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> MarkNotificationsRead()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.MarkNotificationsReadAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get user's saved/bookmarked listings (paginated)</summary>
        /// <remarks>
        /// Returns listings the user has bookmarked using `POST /dashboard/saved-listings`.
        /// Paginated — use `page` and `pageSize` query params.
        /// </remarks>
        [HttpGet("saved-listings")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SavedListingDto>>), 200)]
        public async Task<IActionResult> GetSavedListings([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Dashboard.GetSavedListingsAsync(userId, page, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Bookmark/save a listing</summary>
        /// <remarks>Adds a listing to the user's saved list. Returns 409 if already saved.</remarks>
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

        /// <summary>Get subscription plan details and usage</summary>
        /// <remarks>
        /// Returns the vendor's current subscription plan with listings usage.
        /// - `listingsUsed` — how many listings published
        /// - `listingsRemaining` — how many more can be published
        /// - `usagePercent` — percentage of limit used
        /// - `canUpgrade` — true when usage is at or above 80%
        /// </remarks>
        [HttpGet("subscription")]
        [ProducesResponseType(typeof(ApiResponse<SubscriptionSummaryDto>), 200)]
        public async Task<IActionResult> GetSubscription()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var response = await _service.Dashboard.GetSubscriptionAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
