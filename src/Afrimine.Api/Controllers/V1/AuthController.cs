using Afrimine.Model.ViewModels;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Afrimine.Api.Controllers.V1
{
    [Route("api/v{version:apiversion}/auth")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = Roles.AllUsers)]
    public class AuthController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AuthController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _service.User.RegisterUserAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Logs in a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _service.User.LoginAsync(request);

            if (!response.Success)
                return StatusCode(response.StatusCode, response);

            SetRefreshTokenCookie(response.RefreshToken!);

            return StatusCode(response.StatusCode, response);
        }


        /// <summary>
        /// Confirm Email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("confirm-email")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> ConfirmEmail([FromBody] OtpForCreationDto request)
        {
            var response = await _service.User.ConfirmEmail(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto request)
        {
            var response = await _service.User.ResetPassword(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Forget password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> ForgetPassword([FromBody] ChangeForgotPasswordRequestModel request)
        {
            var response = await _service.User.ChangeForgottenPassword(request);
            return StatusCode(response.StatusCode, response);
        }
        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var response = await _service.User.RefreshTokenAsync(refreshToken, request);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);

            SetRefreshTokenCookie(response.RefreshToken!);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Revoke refresh token (logout)</summary>
        [HttpPost("revoke")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Revoke()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _service.User.RevokeTokenAsync(userId!);
            // Clear the cookie on logout
            Response.Cookies.Delete("refreshToken");

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Resend OTP to email</summary>
        [AllowAnonymous]
        [HttpPost("resend-otp")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDto request)
        {
            var response = await _service.User.ResendOtpAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,                                                    // JS cannot access
                Secure = isProduction,                                              // HTTPS only in production
                SameSite = isProduction ? SameSiteMode.None : SameSiteMode.Lax,   // cross-origin in production
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
    }
}