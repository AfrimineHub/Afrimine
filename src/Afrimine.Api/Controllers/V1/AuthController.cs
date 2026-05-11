using Afrimine.Model.ViewModels;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Afrimine.Api.Controllers.V1
{
    [Route("api/v{version:apiversion}/auth")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AuthController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Registers a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _service.User.LoginAsync(request);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>
        /// Logs in a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("confirm-email")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> ConfirmEmail([FromBody] OtpForCreationDto request)
        {
            var response = await _service.User.ConfirmEmail(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Logs in a user
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
        /// Logs in a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forget-password")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> ForgetPassword([FromBody] ChangeForgotPasswordRequestModel request)
        {
            var response = await _service.User.ChangeForgottenPassword(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}