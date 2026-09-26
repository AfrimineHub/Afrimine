using Afrimine.Services.BL.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using static Afrimine.Services.DTOs.SupplierDto;

namespace Afrimine.Api.Controllers.V1
{
    [Route("api/v{version:apiversion}/webhooks")]
    [ApiVersion("1.0")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IServiceManager _service;

        public WebhookController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>PayScrow payment webhook — called automatically by PayScrow when buyer pays</summary>
        /// <remarks>
        /// **⚠️ Internal use only — do not call this manually.**
        /// PayScrow calls this endpoint automatically when a buyer completes payment.
        ///
        /// **What happens on receipt:**
        /// 1. Booking status → `Active`
        /// 2. Supplier pending wallet balance credited
        /// 3. PayScrow `transactionId` stored for escrow code release later
        ///
        /// Your webhook URL must return HTTP 200. PayScrow retries on failure (30s, 5min, 1hr).
        /// </remarks>
        [HttpPost("payscrow")]
        public async Task<IActionResult> PayscrowWebhook([FromBody] PayscrowWebhookPayload payload)
        {
            await _service.Equipment.HandlePayscrowWebhookAsync(payload);
            return Ok(new { received = true });
        }
    }
}
