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
    [Route("api/v{version:apiversion}/vendor/listings")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = Roles.AllUsers)]
    public class VendorListingsController : ControllerBase
    {
        private readonly IServiceManager _service;

        public VendorListingsController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>Get all listings for the authenticated vendor (paginated)</summary>
        /// <remarks>
        /// Returns the vendor's own listings with filtering options.
        ///
        /// **Status filter values:**
        /// - `Draft` — not yet submitted
        /// - `PendingReview` — submitted, awaiting admin approval
        /// - `Active` — approved and live on marketplace
        /// - `Rejected` — rejected by admin (see adminReviewNote)
        /// - `Flagged` — flagged by admin for review
        /// - `Archived` — soft-deleted
        ///
        /// **CategoryType filter values:**
        /// - `0` = MiningSite
        /// - `1` = MineralSupply
        /// - `2` = Equipment
        /// - `3` = Investment
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<VendorListingListDto>>), 200)]
        public async Task<IActionResult> GetListings([FromQuery] VendorListingQueryDto query)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetListingsAsync(vendorId, query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get a single listing with full details</summary>
        /// <remarks>
        /// Also increments `viewsCount` when called by non-owners.
        /// Returns 403 if the listing belongs to a different vendor.
        /// </remarks>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<VendorListingDetailDto>), 200)]
        public async Task<IActionResult> GetListing(Guid id)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetListingByIdAsync(vendorId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Create a new listing (saved as Draft)</summary>
        /// <remarks>
        /// Creates a listing in `Draft` status. Use `POST /vendor/listings/{id}/publish` to submit for admin review.
        /// Supports both JSON and multipart/form-data (use multipart when including images).
        ///
        /// **CategoryType values:**
        /// - `0` = MiningSite — mining site for sale/lease
        /// - `1` = MineralSupply — minerals for sale
        /// - `2` = Equipment — equipment for sale/rent
        /// - `3` = Investment — investment opportunity
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<VendorListingDetailDto>), 201)]
        [Consumes("multipart/form-data", "application/json")]
        public async Task<IActionResult> CreateListing([FromForm] CreateListingDto request)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.CreateListingAsync(vendorId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Update listing fields</summary>
        /// <remarks>
        /// Partial update — only send fields you want to change.
        /// Cannot edit listings that are `PendingReview` — withdraw first.
        /// </remarks>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<VendorListingDetailDto>), 200)]
        public async Task<IActionResult> UpdateListing(Guid id, [FromBody] UpdateListingDto request)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.UpdateListingAsync(vendorId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Archive/soft-delete a listing</summary>
        /// <remarks>Listing is archived (not permanently deleted). Status changes to `Archived`.</remarks>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteListing(Guid id)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.DeleteListingAsync(vendorId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Upload additional images to a listing</summary>
        [HttpPost("{id:guid}/images")]
        [ProducesResponseType(typeof(ApiResponse<List<ListingImageDto>>), 200)]
        public async Task<IActionResult> UploadImages(Guid id, [FromForm] UploadListingImagesDto request)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.UploadImagesAsync(vendorId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Upload additional images to a listing (max 10 total)</summary>
        /// <remarks>
        /// Use `multipart/form-data` — key name: `images` (multiple files allowed).
        /// First image without a primary is automatically set as primary.
        /// **Accepted formats:** JPG, PNG, WEBP — **Max size:** 10MB each
        /// </remarks>
        [HttpDelete("{id:guid}/images/{imageId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteImage(Guid id, Guid imageId)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.DeleteImageAsync(vendorId, id, imageId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Submit listing for admin review</summary>
        /// <remarks>
        /// Changes status from `Draft` to `PendingReview`. Admin will approve or reject.
        /// You will be notified of the decision. Cannot resubmit while already `PendingReview`.
        /// </remarks>
        [HttpPost("{id:guid}/publish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> PublishListing(Guid id)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.PublishListingAsync(vendorId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get vendor revenue summary with month-over-month changes</summary>

        [HttpGet("/api/v{version:apiVersion}/vendor/revenue/summary")]
        [ProducesResponseType(typeof(ApiResponse<RevenueSummaryDto>), 200)]
        public async Task<IActionResult> GetRevenueSummary()
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetRevenueSummaryAsync(vendorId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get vendor quotes (paginated)</summary>
        /// <remarks>
        /// **status filter:** `pending`, `active`, `accepted`, `rejected`, `expired`
        /// </remarks>
        [HttpGet("/api/v{version:apiVersion}/vendor/quotes")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<VendorQuoteDto>>), 200)]
        public async Task<IActionResult> GetQuotes([FromQuery] VendorQuoteQueryDto query)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetQuotesAsync(vendorId, query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get vendor payout summary — pending and total paid</summary>

        [HttpGet("/api/v{version:apiVersion}/vendor/payout/summary")]
        [ProducesResponseType(typeof(ApiResponse<PayoutSummaryDto>), 200)]
        public async Task<IActionResult> GetPayoutSummary()
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetPayoutSummaryAsync(vendorId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get all vendor orders (paginated)</summary>
        /// <remarks>
        /// **status filter:** `pending`, `ongoing`, `paid`, `delivered`, `completed`, `disputed`, `cancelled`
        /// </remarks>
        [HttpGet("/api/v{version:apiVersion}/vendor/orders")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<VendorOrderDto>>), 200)]
        public async Task<IActionResult> GetOrders([FromQuery] VendorOrderQueryDto query)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetOrdersAsync(vendorId, query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get listing performance metrics (views, saves, inquiries)</summary>
        /// <remarks>
        /// Returns performance data for all vendor listings, ordered by view count (highest first).
        /// Use this to power the listing performance table on the dashboard.
        /// </remarks>
        [HttpGet("/api/v{version:apiVersion}/vendor/dashboard/listings/performance")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<ListingPerformanceItemDto>>), 200)]
        public async Task<IActionResult> GetListingsPerformance([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetListingsPerformanceAsync(vendorId, page, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get vendor's full dashboard in a single call</summary>
        /// <remarks>
        /// Aggregates subscription, revenue, stats, listing performance, and recent notifications.
        /// Use this to power the entire vendor dashboard with one API call instead of multiple.
        /// </remarks>
        [HttpGet("/api/v{version:apiVersion}/vendor/dashboard")]
        [ProducesResponseType(typeof(ApiResponse<VendorDashboardDto>), 200)]
        public async Task<IActionResult> GetVendorDashboard()
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetVendorDashboardAsync(vendorId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
