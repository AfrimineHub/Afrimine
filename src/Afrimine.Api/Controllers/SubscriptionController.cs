using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Shared.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Afrimine.Api.Controllers
{
    [Route("api/v{version:apiversion}/subscription")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IServiceManager _service;

        public SubscriptionController(IServiceManager service)
        {
            _service = service;
        }
        /// <summary>Get all subscription plans (public)</summary>
        /// <remarks>
        /// Returns the full plan catalog.
        ///
        /// **Plan IDs:** `free`, `bronze`, `silver`, `gold`, `diamond`, `platinum`
        ///
        /// Use `POST /subscription/checkout` with the chosen `planId` to start payment.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("plans")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubscriptionPlanDto>>), 200)]
        public async Task<IActionResult> GetPlans()
        {
            var response = await _service.Subscription.GetPlansAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Start a payment checkout session for a subscription plan</summary>
        /// <remarks>
        /// Creates a PayStack checkout session and returns a `checkoutUrl`.
        /// Redirect the user to `checkoutUrl` to complete payment.
        /// After payment, verify using `GET /subscription/checkout/verify?sessionId={sessionId}`.
        ///
        /// **Example request:**
        /// ```json
        /// {
        ///   "planId": "gold",
        ///   "successUrl": "https://app.afrimine.com/dashboard/my-subscription?checkout=success",
        ///   "cancelUrl": "https://app.afrimine.com/dashboard/my-subscription?checkout=canceled"
        /// }
        /// ```
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("checkout")]
        [ProducesResponseType(typeof(ApiResponse<CheckoutResponseDto>), 200)]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Subscription.CheckoutAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }
        /// <summary>Verify payment after redirect from checkout page</summary>
        /// <remarks>
        /// Call this when user lands on the `successUrl` after payment.
        /// Pass the `sessionId` from the checkout response to confirm the payment and activate the plan.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("checkout/verify")]
        [ProducesResponseType(typeof(ApiResponse<SubscriptionSummaryDto>), 200)]
        public async Task<IActionResult> VerifyCheckout([FromQuery] string sessionId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Subscription.VerifyCheckoutAsync(userId, sessionId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Downgrade to free plan immediately</summary>
        /// <remarks>
        /// Switches the vendor to the free plan immediately.
        /// Returns updated subscription with `status: "canceled"` and `canceledAt` timestamp.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("change-plan")]
        [ProducesResponseType(typeof(ApiResponse<SubscriptionSummaryDto>), 200)]
        public async Task<IActionResult> ChangePlan([FromBody] ChangePlanDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Subscription.ChangePlanAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Cancel subscription at end of billing period</summary>
        /// <remarks>
        /// Sets subscription to cancel at the end of the current period (`renewsAt`).
        /// Vendor retains access until `renewsAt` date.
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpPost("cancel")]
        [ProducesResponseType(typeof(ApiResponse<SubscriptionSummaryDto>), 200)]
        public async Task<IActionResult> Cancel([FromBody] CancelSubscriptionDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Subscription.CancelAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get billing invoice history (paginated)</summary>
        /// <remarks>
        /// Returns all past subscription invoices.
        ///
        /// **InvoiceStatus values:** `pending`, `paid`, `failed`, `refunded`
        /// </remarks>
        [Authorize(Roles = Roles.Vendor)]
        [HttpGet("invoices")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<InvoiceDto>>), 200)]
        public async Task<IActionResult> GetInvoices([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Subscription.GetInvoicesAsync(userId, page, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Submit a custom/enterprise plan inquiry to the sales team</summary>
        /// <remarks>
        /// For platinum and custom plans. The sales team will contact you within 24 hours.
        ///
        /// **preferredPlanId:** `platinum` or `custom`
        /// </remarks>
        [Authorize(Roles = Roles.AllUsers)]
        [HttpPost("contact-sales")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ContactSales([FromBody] ContactSalesDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Subscription.ContactSalesAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
