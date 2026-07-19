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

        /// <summary>PayScrow payment webhook — called when miner completes payment</summary>
        [HttpPost("payscrow")]
        public async Task<IActionResult> PayscrowWebhook([FromBody] PayscrowWebhookPayload payload)
        {
            await _service.Equipment.HandlePayscrowWebhookAsync(payload);
            return Ok(new { received = true });
        }
    }
}
