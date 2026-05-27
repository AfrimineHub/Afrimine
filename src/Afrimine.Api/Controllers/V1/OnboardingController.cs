using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Shared.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Afrimine.Api.Controllers.V1
{
    [Route("api/v{version:apiversion}/onboarding")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = Roles.AllUsers)]
    public class OnboardingController : ControllerBase
    {
        private readonly IServiceManager _service;

        public OnboardingController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>Step 2 — Business profile setup</summary>
        [HttpPost("business-profile")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SetupBusinessProfile([FromBody] BusinessProfileDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _service.User.SetupBusinessProfileAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Step 3 — KYC document upload</summary>
        [HttpPost("kyc")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> UploadKyc([FromForm] KycUploadDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var response = await _service.User.UploadKycAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get onboarding profile and current step</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<VendorProfileResponseDto>), 200)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.User.GetLoggedInUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();
            var response = await _service.User.GetVendorProfileAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
