using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Shared.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Afrimine.Services.DTOs.AuthDto;

namespace Afrimine.Api.Controllers.V1
{
    [Route("api/v{version:apiversion}/auth")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UsersController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>Get the currently authenticated user's profile</summary>
        /// <remarks>
        /// Returns the logged-in user's full profile including role, status, and account details.
        /// Works for all roles — Vendor, Buyer, Investor, Support, SuperAdmin.
        ///
        /// **Use this to:**
        /// - Determine which dashboard to show (vendor vs buyer vs admin)
        /// - Check if KYC is verified before allowing listing creation
        /// - Display user info in the app header/nav
        ///
        /// **AccountStatus values:**
        /// - `Pending` — registered but email not yet confirmed
        /// - `Active` — fully active account
        /// - `Suspended` — temporarily suspended by admin
        /// - `Banned` — permanently banned
        /// </remarks>
        [HttpGet("current")]
        [Authorize(Roles = Roles.AllUsers)]
        [ProducesResponseType(typeof(ApiResponse<CurrentUserDto>), 200)]
        public async Task<IActionResult> Current()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            var response = await _service.User.GetCurrentUser(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Upload/replace the current user's profile photo</summary>
        /// <remarks>Send as multipart/form-data with a `photo` file field. Replaces any existing photo.</remarks>
        [HttpPost("profile-photo")]
        [Authorize(Roles = Roles.AllUsers)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> UploadProfilePhoto([FromForm] ProfilePhotoUploadDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.User.UploadProfilePhotoAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
