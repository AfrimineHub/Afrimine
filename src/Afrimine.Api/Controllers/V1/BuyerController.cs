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
    [Route("api/v{version:apiversion}/buyer")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = Roles.Buyer)]
    public class BuyerController : ControllerBase
    {
        private readonly IServiceManager _service;

        public BuyerController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>Get buyer dashboard summary</summary>
        /// <remarks>
        /// Returns counts for the buyer's dashboard top stats bar.
        /// - `savedListingsCount` — bookmarked listings
        /// - `unreadMessagesCount` — unread messages
        /// - `ongoingOrdersCount` — active bookings/orders
        /// - `openRfqsCount` — RFQs still open for vendor quotes
        /// </remarks>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<BuyerDashboardSummaryDto>), 200)]
        public async Task<IActionResult> GetBuyerSummary()
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            return StatusCode(200, await _service.Buyer.GetBuyerSummaryAsync(buyerId));
        }

        /// <summary>Get buyer's orders (paginated)</summary>
        /// <remarks>
        /// **Status filter values:**
        /// - `0` = Pending — order placed, awaiting payment
        /// - `1` = Ongoing — in progress
        /// - `2` = Paid — payment confirmed, in escrow
        /// - `3` = Delivered — supplier marked as delivered
        /// - `4` = Completed — buyer confirmed delivery, funds released
        /// - `5` = Disputed — dispute raised
        /// - `6` = Frozen — frozen by admin
        /// - `7` = Cancelled
        /// </remarks>
        [HttpGet("orders")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<BuyerOrderDto>>), 200)]
        public async Task<IActionResult> GetOrders([FromQuery] BuyerOrderQueryDto query)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.GetOrdersAsync(buyerId, query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get single order detail</summary>
        [HttpGet("orders/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<BuyerOrderDto>), 200)]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.GetOrderByIdAsync(buyerId, id);
            return StatusCode(response.StatusCode, response);
        }
        /// <summary>Confirm delivery — releases funds to vendor</summary>
        /// <remarks>
        /// Marks the order as `Delivered`. Order must be in `Paid` status.
        /// Use this when you have physically received the goods/equipment.
        /// After confirmation, vendor can request withdrawal of their funds.
        /// </remarks>
        [HttpPatch("orders/{id:guid}/confirm-delivery")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ConfirmDelivery(Guid id)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.ConfirmDeliveryAsync(buyerId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Raise a dispute on an order</summary>
        /// <remarks>
        /// Opens a dispute that freezes the escrow until admin resolves it.
        /// Cannot dispute completed or cancelled orders.
        /// Provide a clear reason — admin will review within 24–48 hours.
        /// </remarks>
        [HttpPost("orders/{id:guid}/dispute")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DisputeOrder(Guid id, [FromBody] DisputeOrderDto request)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.DisputeOrderAsync(buyerId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Initiate escrow payment for an order</summary>
        /// <remarks>
        /// Records the payment reference once the buyer completes payment through your payment gateway.
        /// Order status changes to `Paid` and funds are placed in escrow.
        /// </remarks>
        [HttpPost("orders/{id:guid}/pay")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> PayOrder(Guid id, [FromBody] PayOrderDto request)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.PayOrderAsync(buyerId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>List all buyer RFQs</summary>
        /// <remarks>
        /// Returns RFQs created by the authenticated buyer.
        /// Each RFQ includes `responseCount` — number of vendor quotes received.
        ///
        /// **Status values:** Open, Closed, Awarded, Cancelled
        /// </remarks>
        [HttpGet("rfqs")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<RfqDto>>), 200)]
        public async Task<IActionResult> GetBuyerRfqs([FromQuery] RfqQueryDto query)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.GetBuyerRfqsAsync(buyerId, query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Create a new RFQ (Request for Quote)</summary>
        /// <remarks>
        /// Post a buying requirement — vendors can see and respond with quotes.
        /// RFQ expires after `expiresAt` date (default: 30 days from now).
        ///
        /// **Example:**
        /// ```json
        /// {
        ///   "title": "Need 50 tons of Gold Ore",
        ///   "description": "High-grade gold ore, minimum 80% purity",
        ///   "mineralType": "Gold",
        ///   "quantity": "50",
        ///   "unit": "tons",
        ///   "targetPrice": "₦5,000,000",
        ///   "location": "Jos, Plateau State",
        ///   "country": "Nigeria"
        /// }
        /// ```
        /// </remarks>
        [HttpPost("rfqs")]
        [ProducesResponseType(typeof(ApiResponse<RfqDto>), 201)]
        public async Task<IActionResult> CreateRfq([FromBody] CreateRfqDto request)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.CreateRfqAsync(buyerId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get a single RFQ with vendor quote responses</summary>

        [HttpGet("rfqs/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<RfqDto>), 200)]
        public async Task<IActionResult> GetRfq(Guid id)
        {
            var response = await _service.Buyer.GetRfqByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
