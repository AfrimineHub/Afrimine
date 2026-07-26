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


        /// <summary>Get escrow status for a buyer order</summary>
        /// <remarks>
        /// Returns the current state of the escrow account for this order.
        ///
        /// **EscrowStatus values:**
        /// - `Pending` — escrow created, awaiting buyer payment
        /// - `Funded` — buyer paid, funds held in escrow
        /// - `Released` — funds released to vendor after delivery confirmation
        /// - `Frozen` — frozen by admin pending dispute resolution
        /// - `Refunded` — funds returned to buyer
        /// </remarks>
        
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

        /// <summary>Initiate escrow payment checkout for an order (Buyer only)</summary>
        /// <remarks>
        /// Creates a PayScrow checkout session and returns a `checkoutUrl`.
        /// Redirect the buyer to `checkoutUrl` to complete payment.
        ///
        /// **After payment:**
        /// 1. PayScrow calls the webhook automatically → escrow status changes to `Funded`
        /// 2. Vendor is notified to proceed with delivery
        /// 3. Call `GET /buyer/orders/{id}/checkout/verify` to confirm payment on your end
        /// </remarks>
        
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

        /// <summary>Verify escrow payment after redirect from checkout page (Buyer only)</summary>
        /// <remarks>
        /// Call this when the buyer lands back on your app after completing payment on PayScrow.
        /// Pass the `sessionId` from the checkout response to verify payment was successful.
        /// Escrow status changes to `Funded` and order status changes to `Paid`.
        /// </remarks>
        
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

        /// <summary>Get vendor quotes submitted for a buyer's RFQ (Buyer only)</summary>
        /// <remarks>
        /// Returns all vendor quotes for a specific RFQ created by the authenticated buyer.
        /// Use `POST /buyer/rfqs/{rfqId}/quotes/{quoteId}/accept` to accept a quote and create an order.
        ///
        /// **QuoteStatus values:**
        /// - `Pending` — submitted by vendor, awaiting buyer review
        /// - `Accepted` — buyer accepted → order created automatically
        /// - `Rejected` — buyer rejected this quote
        /// - `Expired` — quote passed expiry date
        /// </remarks>
        
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

        /// <summary>Accept a vendor quote — automatically creates an order (Buyer only)</summary>
        /// <remarks>
        /// Accepts the selected vendor quote. This automatically:
        /// 1. Creates a new Order linked to the RFQ and vendor
        /// 2. Changes RFQ status to `Awarded`
        /// 3. Closes the RFQ (no more quotes accepted)
        ///
        /// After accepting, proceed to `POST /buyer/orders/{orderId}/checkout` to fund the escrow.
        /// </remarks>
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

        /// <summary>Get escrow status for a vendor's order (Vendor only)</summary>
        /// <remarks>
        /// Same as the buyer escrow status but accessible to the vendor side.
        /// Shows whether the buyer has funded the escrow and when funds will be released.
        /// </remarks>
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

        /// <summary>Mark an order as delivered (Vendor only)</summary>
        /// <remarks>
        /// Vendor confirms they have fulfilled the order (goods delivered / service completed).
        /// Order must be in `Paid` status (buyer has funded escrow).
        /// After marking delivered, buyer can confirm receipt to release funds.
        /// Order status changes to `Delivered`.
        /// </remarks>
        
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

        /// <summary>Submit a quote in response to a buyer RFQ (Vendor only)</summary>
        /// <remarks>
        /// Vendors can browse open RFQs at `GET /rfqs?Status=open` and submit quotes.
        /// Buyer will see the quote in `GET /buyer/rfqs/{id}/quotes`.
        ///
        /// **Example request:**
        /// ```json
        /// {
        ///   "amount": 5000000,
        ///   "currency": "NGN",
        ///   "note": "Can deliver 50 tons of gold ore within 2 weeks. Grade A quality."
        /// }
        /// ```
        /// </remarks>
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

        /// <summary>Request a payout/withdrawal to bank account (Vendor only)</summary>
        /// <remarks>
        /// Requests a transfer from the vendor's available wallet balance to their registered bank account.
        /// Admin reviews and approves/rejects withdrawals — check status at `GET /admin/withdrawals`.
        /// Processing takes 24–48 hours after admin approval.
        ///
        /// **Note:** Only `availableBalance` can be withdrawn. `pendingBalance` is locked in escrow.
        /// </remarks>

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


        /// <summary>Get all disputes queue (SuperAdmin only)</summary>
        /// <remarks>
        /// Returns all open disputes across the platform for admin review.
        ///
        /// **status filter values:**
        /// - `Open` — newly raised, needs attention
        /// - `UnderReview` — being investigated by admin
        /// - `ResolvedBuyer` — resolved in favour of the buyer (escrow refunded)
        /// - `ResolvedVendor` — resolved in favour of the vendor (escrow released)
        /// - `Closed` — dispute closed
        /// </remarks>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("api/v{version:apiVersion}/admin/disputes")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<DisputeDto>>), 200)]
        public async Task<IActionResult> GetDisputes([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
        {
            var response = await _service.Escrow.GetDisputesAsync(page, pageSize, status);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Get full dispute details (SuperAdmin only)</summary>
        /// <remarks>
        /// Returns dispute info including the order, listing, buyer and vendor details, and the reason.
        /// </remarks>

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("api/v{version:apiVersion}/admin/disputes/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<DisputeDto>), 200)]
        public async Task<IActionResult> GetDispute(Guid id)
        {
            var response = await _service.Escrow.GetDisputeByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Resolve a dispute in favour of buyer or vendor (SuperAdmin only)</summary>
        /// <remarks>
        /// Admin reviews evidence and resolves the dispute.
        ///
        /// **resolution values:**
        /// - `"buyer"` — refunds escrow to buyer
        /// - `"vendor"` — releases escrow to vendor
        ///
        /// **Example request:**
        /// ```json
        /// {
        ///   "resolution": "vendor",
        ///   "adminNote": "Vendor provided proof of delivery. Releasing funds."
        /// }
        /// ```
        /// </remarks>

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

        /// <summary>Freeze all funds for an order (SuperAdmin only)</summary>
        /// <remarks>
        /// Emergency action — freezes the escrow account for an order.
        /// Use when fraud is suspected or a serious dispute is raised.
        /// Order status changes to `Frozen`. Neither buyer nor vendor can access funds.
        /// Unfreeze by resolving the dispute.
        /// </remarks>

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

        /// <summary>Manually release funds to vendor (SuperAdmin only)</summary>
        /// <remarks>
        /// Admin manually releases escrow funds to the vendor.
        /// Use when buyer has confirmed delivery but funds are stuck, or after resolving a dispute in vendor's favour.
        /// Order status changes to `Completed`. Escrow status changes to `Released`.
        /// </remarks>

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
