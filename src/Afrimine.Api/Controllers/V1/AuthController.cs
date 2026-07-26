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

        /// <summary>Register a new user account</summary>
        /// <remarks>
        /// Creates a new account. After registration an OTP is sent to your email — verify it using `POST /auth/confirm-email`.
        ///
        /// **Role values:**
        /// - `1` = Vendor — sells equipment, minerals, or manpower on the platform
        /// - `2` = Buyer — purchases or rents from vendors
        /// - `3` = Investor — invests in mining opportunities
        /// - `4` = Support — internal support staff
        ///
        /// **Password requirements:** Minimum 8 characters, must include uppercase, lowercase, digit, and special character.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _service.User.RegisterUserAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Login and get access token</summary>
        /// <remarks>
        /// Authenticates the user and returns a JWT access token + sets an `HttpOnly` refresh token cookie.
        ///
        /// **All roles use this same endpoint.** The token contains your role automatically.
        ///
        /// **How to use the token:**
        /// 1. Copy the `token` value from the response
        /// 2. Click **Authorize** at the top of Swagger
        /// 3. Enter: `Bearer {token}`
        /// 4. All protected endpoints will use it automatically
        ///
        /// **Token expiry:** 1 hour. Use `POST /auth/refresh-token` to get a new one without logging in again.
        /// </remarks>
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


        /// <summary>Verify email address using OTP sent after registration</summary>
        /// <remarks>
        /// Enter the 6-digit OTP sent to your email address during registration.
        /// Account cannot be used until email is confirmed.
        /// OTP expires after 10 minutes — use `POST /auth/resend-otp` to get a new one.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("confirm-email")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> ConfirmEmail([FromBody] OtpForCreationDto request)
        {
            var response = await _service.User.ConfirmEmail(request);
            return StatusCode(response.StatusCode, response);
        }
        /// <summary>Set a new password using the reset OTP</summary>
        /// <remarks>
        /// Complete the password reset flow using the OTP received via email.
        /// Provide your email, the OTP, and your new password.
        ///
        /// **New password requirements:** Minimum 8 characters, uppercase, lowercase, digit, and special character.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto request)
        {
            var response = await _service.User.ResetPassword(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Request a password reset link</summary>
        /// <remarks>
        /// Sends a password reset OTP/link to the user's registered email address.
        /// After receiving it, use `POST /auth/reset-password` to set a new password.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> ForgetPassword([FromBody] ChangeForgotPasswordRequestModel request)
        {
            var response = await _service.User.ChangeForgottenPassword(request);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Refresh the access token using the refresh token cookie</summary>
        /// <remarks>
        /// Returns a new JWT access token without requiring the user to log in again.
        /// The refresh token is stored as an `HttpOnly` cookie — it is sent automatically by the browser.
        ///
        /// **When to call this:** When you receive a `401 Unauthorized` response, call this endpoint to get a new token,
        /// then retry the original request with the new token.
        ///
        /// **Refresh token expiry:** 7 days.
        /// </remarks>
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


        /// <summary>Logout — revoke refresh token and clear cookie</summary>
        /// <remarks>
        /// Invalidates the current refresh token and clears the `refreshToken` cookie.
        /// The access token will still be valid until it expires (up to 1 hour) —
        /// the frontend should also discard the token from local storage on logout.
        /// </remarks>
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

        /// <summary>Resend OTP verification email</summary>
        /// <remarks>
        /// Sends a new 6-digit OTP to the provided email address.
        /// Use this if the original OTP expired or was not received.
        /// </remarks>

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