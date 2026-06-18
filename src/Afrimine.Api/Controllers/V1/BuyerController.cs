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
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<BuyerDashboardSummaryDto>), 200)]
        public async Task<IActionResult> GetBuyerSummary()
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            return StatusCode(200, await _service.Buyer.GetBuyerSummaryAsync(buyerId));
        }

        /// <summary>Get buyer orders (paginated)</summary>
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

        /// <summary>Confirm delivery — must be in Paid status</summary>
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
        [HttpPost("orders/{id:guid}/dispute")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DisputeOrder(Guid id, [FromBody] DisputeOrderDto request)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.DisputeOrderAsync(buyerId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Pay for an order (escrow initiation)</summary>
        [HttpPost("orders/{id:guid}/pay")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> PayOrder(Guid id, [FromBody] PayOrderDto request)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.PayOrderAsync(buyerId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>List buyer RFQs</summary>
        [HttpGet("rfqs")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<RfqDto>>), 200)]
        public async Task<IActionResult> GetBuyerRfqs([FromQuery] RfqQueryDto query)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.GetBuyerRfqsAsync(buyerId, query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Create a new RFQ</summary>
        [HttpPost("rfqs")]
        [ProducesResponseType(typeof(ApiResponse<RfqDto>), 201)]
        public async Task<IActionResult> CreateRfq([FromBody] CreateRfqDto request)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.CreateRfqAsync(buyerId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get a single RFQ by id</summary>
        [HttpGet("rfqs/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<RfqDto>), 200)]
        public async Task<IActionResult> GetRfq(Guid id)
        {
            var response = await _service.Buyer.GetRfqByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
