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

        /// <summary>Get paginated listings for the authenticated vendor</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<VendorListingListDto>>), 200)]
        public async Task<IActionResult> GetListings([FromQuery] VendorListingQueryDto query)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetListingsAsync(vendorId, query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get single listing detail</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<VendorListingDetailDto>), 200)]
        public async Task<IActionResult> GetListing(Guid id)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetListingByIdAsync(vendorId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Create a new listing — JSON or multipart when images included</summary>
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
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<VendorListingDetailDto>), 200)]
        public async Task<IActionResult> UpdateListing(Guid id, [FromBody] UpdateListingDto request)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.UpdateListingAsync(vendorId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Soft-delete / archive a listing</summary>
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

        /// <summary>Remove an image from a listing</summary>
        [HttpDelete("{id:guid}/images/{imageId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteImage(Guid id, Guid imageId)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.DeleteImageAsync(vendorId, id, imageId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Submit a draft listing for admin review</summary>
        [HttpPost("{id:guid}/publish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> PublishListing(Guid id)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.PublishListingAsync(vendorId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get revenue summary for the authenticated vendor</summary>
        [HttpGet("/api/v{version:apiVersion}/vendor/revenue/summary")]
        [ProducesResponseType(typeof(ApiResponse<RevenueSummaryDto>), 200)]
        public async Task<IActionResult> GetRevenueSummary()
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetRevenueSummaryAsync(vendorId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get vendor quotes</summary>
        [HttpGet("/api/v{version:apiVersion}/vendor/quotes")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<VendorQuoteDto>>), 200)]
        public async Task<IActionResult> GetQuotes([FromQuery] VendorQuoteQueryDto query)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetQuotesAsync(vendorId, query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get vendor payout summary</summary>
        [HttpGet("/api/v{version:apiVersion}/vendor/payout/summary")]
        [ProducesResponseType(typeof(ApiResponse<PayoutSummaryDto>), 200)]
        public async Task<IActionResult> GetPayoutSummary()
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetPayoutSummaryAsync(vendorId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get vendor orders</summary>
        [HttpGet("/api/v{version:apiVersion}/vendor/orders")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<VendorOrderDto>>), 200)]
        public async Task<IActionResult> GetOrders([FromQuery] VendorOrderQueryDto query)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetOrdersAsync(vendorId, query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get listing performance metrics</summary>
        [HttpGet("/api/v{version:apiVersion}/vendor/dashboard/listings/performance")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<ListingPerformanceItemDto>>), 200)]
        public async Task<IActionResult> GetListingsPerformance([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var vendorId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(vendorId)) return Unauthorized();

            var response = await _service.VendorListing.GetListingsPerformanceAsync(vendorId, page, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get full vendor dashboard — aggregated in one call</summary>
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
