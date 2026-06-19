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

        /// <summary>Get plan catalog (public)</summary>
        [AllowAnonymous]
        [HttpGet("plans")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubscriptionPlanDto>>), 200)]
        public async Task<IActionResult> GetPlans()
        {
            var response = await _service.Subscription.GetPlansAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Start payment checkout session</summary>
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

        /// <summary>Verify payment after redirect</summary>
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

        /// <summary>Switch to free / downgrade plan</summary>
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

        /// <summary>Cancel subscription at period end</summary>
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

        /// <summary>Get billing invoice history</summary>
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

        /// <summary>Contact sales for custom plan</summary>
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
