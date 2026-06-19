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
    [ApiVersion("1.0")]
    [ApiController]
    public class EscrowController : ControllerBase
    {
        private readonly IServiceManager _service;

        public EscrowController(IServiceManager service)
        {
            _service = service;
        }

        // ── Buyer ──────────────────────────────────────────────────────────────

        /// <summary>Get escrow status for buyer order</summary>
        [Authorize(Roles = Roles.Buyer)]
        [HttpGet("api/v{version:apiVersion}/buyer/orders/{id:guid}/escrow")]
        [ProducesResponseType(typeof(ApiResponse<EscrowStatusDto>), 200)]
        public async Task<IActionResult> GetEscrowStatus(Guid id)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Escrow.GetEscrowStatusAsync(userId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Fund escrow — initiate Paystack checkout</summary>
        [Authorize(Roles = Roles.Buyer)]
        [HttpPost("api/v{version:apiVersion}/buyer/orders/{id:guid}/checkout")]
        [ProducesResponseType(typeof(ApiResponse<EscrowCheckoutResponseDto>), 200)]
        public async Task<IActionResult> CheckoutEscrow(Guid id)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Escrow.CheckoutEscrowAsync(userId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Verify escrow payment after Paystack redirect</summary>
        [Authorize(Roles = Roles.Buyer)]
        [HttpGet("api/v{version:apiVersion}/buyer/orders/{id:guid}/checkout/verify")]
        [ProducesResponseType(typeof(ApiResponse<EscrowStatusDto>), 200)]
        public async Task<IActionResult> VerifyEscrow(Guid id, [FromQuery] string sessionId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Escrow.VerifyEscrowPaymentAsync(userId, id, sessionId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get RFQ quotes (buyer)</summary>
        [Authorize(Roles = Roles.Buyer)]
        [HttpGet("api/v{version:apiVersion}/buyer/rfqs/{rfqId:guid}/quotes")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RfqQuoteDto>>), 200)]
        public async Task<IActionResult> GetRfqQuotes(Guid rfqId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Escrow.GetRfqQuotesAsync(userId, rfqId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Accept a quote — creates order</summary>
        [Authorize(Roles = Roles.Buyer)]
        [HttpPost("api/v{version:apiVersion}/buyer/rfqs/{rfqId:guid}/quotes/{quoteId:guid}/accept")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> AcceptQuote(Guid rfqId, Guid quoteId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Escrow.AcceptRfqQuoteAsync(userId, rfqId, quoteId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Vendor ─────────────────────────────────────────────────────────────

        /// <summary>Get vendor order detail</summary>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("api/v{version:apiVersion}/vendor/orders/{id:guid}/escrow")]
        [ProducesResponseType(typeof(ApiResponse<EscrowStatusDto>), 200)]
        public async Task<IActionResult> GetVendorEscrowStatus(Guid id)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Escrow.GetVendorEscrowStatusAsync(userId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Mark order as delivered (vendor)</summary>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPatch("api/v{version:apiVersion}/vendor/orders/{id:guid}/mark-delivered")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> MarkDelivered(Guid id)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Escrow.MarkDeliveredAsync(userId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Submit a quote for an RFQ (vendor)</summary>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("api/v{version:apiVersion}/vendor/rfqs/{rfqId:guid}/quotes")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SubmitQuote(Guid rfqId, [FromBody] VendorQuoteSubmitDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Escrow.SubmitRfqQuoteAsync(userId, rfqId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Request payout withdrawal (vendor)</summary>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("api/v{version:apiVersion}/vendor/payout/request")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RequestPayout([FromBody] PayoutRequestDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Escrow.RequestPayoutAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        // ── Admin ──────────────────────────────────────────────────────────────

        /// <summary>Get dispute queue (admin)</summary>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("api/v{version:apiVersion}/admin/disputes")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<DisputeDto>>), 200)]
        public async Task<IActionResult> GetDisputes([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
        {
            var response = await _service.Escrow.GetDisputesAsync(page, pageSize, status);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get dispute detail (admin)</summary>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("api/v{version:apiVersion}/admin/disputes/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<DisputeDto>), 200)]
        public async Task<IActionResult> GetDispute(Guid id)
        {
            var response = await _service.Escrow.GetDisputeByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Resolve dispute (admin)</summary>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPost("api/v{version:apiVersion}/admin/disputes/{id:guid}/resolve")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ResolveDispute(Guid id, [FromBody] ResolveDisputeDto request)
        {
            var adminId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(adminId)) return Unauthorized();
            var response = await _service.Escrow.ResolveDisputeAsync(adminId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Freeze order funds (admin)</summary>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPost("api/v{version:apiVersion}/admin/orders/{id:guid}/freeze")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> FreezeOrder(Guid id)
        {
            var adminId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(adminId)) return Unauthorized();
            var response = await _service.Escrow.FreezeOrderAsync(adminId, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Release funds to vendor (admin)</summary>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPost("api/v{version:apiVersion}/admin/orders/{id:guid}/release-funds")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ReleaseFunds(Guid id)
        {
            var adminId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(adminId)) return Unauthorized();
            var response = await _service.Escrow.ReleaseFundsAsync(adminId, id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
