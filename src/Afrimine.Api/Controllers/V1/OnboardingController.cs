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

        /// <summary>Setup business profile (Onboarding Step 1 of 3)</summary>
        /// <remarks>
        /// Save business type, country, state, office address and website.
        /// Must be completed before uploading KYC documents.
        ///
        /// **BusinessType values:**
        /// - `1` = Individual
        /// - `2` = Company
        /// - `3` = Cooperative
        /// - `4` = GovernmentEntity
        /// </remarks>
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

        /// <summary>Upload KYC verification document (Onboarding Step 2 of 3)</summary>
        /// <remarks>
        /// Upload one of the accepted identity/business documents.
        /// Use `multipart/form-data` — key name: `file`
        /// **Accepted formats:** PDF, JPG, PNG — **Max size:** 10MB
        ///
        /// **DocumentType values:**
        /// - `1` = GovernmentIdOrPassport
        /// - `2` = CompanyRegistrationCertificate
        /// - `3` = MiningLicense
        /// - `4` = ExportLicense
        /// - `5` = MineralLicense
        /// </remarks>

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

        /// <summary>Get current vendor onboarding profile and KYC status</summary>
        /// <remarks>
        /// Returns the vendor's current profile including KYC verification status.
        ///
        /// **KycStatus values:**
        /// - `NotStarted` — no document uploaded yet
        /// - `Pending` — document uploaded, awaiting admin review
        /// - `Verified` — KYC approved, full platform access
        /// - `Rejected` — rejected with reason, resubmit required
        /// </remarks>
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
