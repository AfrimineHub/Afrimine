using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Shared.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Get current logged in user
        /// </summary>
        /// <returns></returns>
        [HttpGet("current")]
        //[Authorize]
        [Authorize(Roles = "Vendor,Buyer,Investor,Support,SuperAdmin")]
        [ProducesResponseType(typeof(ApiResponse<CurrentUserDto>), 200)]
        public async Task<IActionResult> Current()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            var response = await _service.User.GetCurrentUser(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
