using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Afrimine.Api.Controllers
{
    [Route("api/v{version:apiversion}/admin")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AdminController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>Admin dashboard</summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<AdminDashboardDto>), 200)]
        public async Task<IActionResult> GetDashboard()
        {
            var response = await _service.Admin.GetDashboardAsync();
            return StatusCode(response.StatusCode, response);
        }

        // ── Users ──────────────────────────────────────────────────────────────

        [HttpGet("users")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminUserListItemDto>>), 200)]
        public async Task<IActionResult> GetUsers([FromQuery] AdminUserQueryDto query)
        {
            var response = await _service.Admin.GetUsersAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("users/stats")]
        [ProducesResponseType(typeof(ApiResponse<AdminUserStatsDto>), 200)]
        public async Task<IActionResult> GetUserStats()
        {
            var response = await _service.Admin.GetUserStatsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("users/{userId}/suspend")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SuspendUser(string userId, [FromBody] AdminUserActionDto request)
        {
            var response = await _service.Admin.SuspendUserAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("users/{userId}/ban")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> BanUser(string userId, [FromBody] AdminUserActionDto request)
        {
            var response = await _service.Admin.BanUserAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("users/{userId}/reactivate")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ReactivateUser(string userId)
        {
            var response = await _service.Admin.ReactivateUserAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Listings ───────────────────────────────────────────────────────────

        [HttpGet("listings")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminListingListItemDto>>), 200)]
        public async Task<IActionResult> GetListings([FromQuery] AdminListingQueryDto query)
        {
            var response = await _service.Admin.GetListingsAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("listings/counts")]
        [ProducesResponseType(typeof(ApiResponse<AdminListingCountsDto>), 200)]
        public async Task<IActionResult> GetListingCounts()
        {
            var response = await _service.Admin.GetListingCountsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("listings/{id:guid}/approve")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ApproveListing(Guid id)
        {
            var response = await _service.Admin.ApproveListingAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("listings/{id:guid}/reject")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RejectListing(Guid id, [FromBody] AdminListingActionDto request)
        {
            var response = await _service.Admin.RejectListingAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("listings/{id:guid}/flag")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> FlagListing(Guid id, [FromBody] AdminListingActionDto request)
        {
            var response = await _service.Admin.FlagListingAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("listings/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteListing(Guid id)
        {
            var response = await _service.Admin.DeleteListingAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        // ── Quotes ─────────────────────────────────────────────────────────────

        [HttpGet("quotes")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminQuoteListItemDto>>), 200)]
        public async Task<IActionResult> GetQuotes([FromQuery] AdminQuoteQueryDto query)
        {
            var response = await _service.Admin.GetQuotesAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        // ── Orders ─────────────────────────────────────────────────────────────

        [HttpGet("orders")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminOrderListItemDto>>), 200)]
        public async Task<IActionResult> GetOrders([FromQuery] AdminOrderQueryDto query)
        {
            var response = await _service.Admin.GetOrdersAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("orders/summary")]
        [ProducesResponseType(typeof(ApiResponse<AdminOrderSummaryDto>), 200)]
        public async Task<IActionResult> GetOrderSummary([FromQuery] string? q)
        {
            var response = await _service.Admin.GetOrderSummaryAsync(q);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("orders/{orderId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<AdminOrderDetailDto>), 200)]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            var response = await _service.Admin.GetOrderDetailAsync(orderId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Revenue ────────────────────────────────────────────────────────────

        [HttpGet("revenue")]
        [ProducesResponseType(typeof(ApiResponse<AdminRevenueSummaryDto>), 200)]
        public async Task<IActionResult> GetRevenue()
        {
            var response = await _service.Admin.GetRevenueSummaryAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("revenue/transactions")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminTransactionItemDto>>), 200)]
        public async Task<IActionResult> GetTransactions([FromQuery] AdminTransactionQueryDto query)
        {
            var response = await _service.Admin.GetTransactionsAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        // ── Withdrawals ────────────────────────────────────────────────────────

        [HttpGet("withdrawals")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminWithdrawalItemDto>>), 200)]
        public async Task<IActionResult> GetWithdrawals([FromQuery] AdminWithdrawalQueryDto query)
        {
            var response = await _service.Admin.GetWithdrawalsAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("withdrawals/{id:guid}/approve")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ApproveWithdrawal(Guid id)
        {
            var response = await _service.Admin.ApproveWithdrawalAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("withdrawals/{id:guid}/hold")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> HoldWithdrawal(Guid id, [FromBody] AdminWithdrawalActionDto request)
        {
            var response = await _service.Admin.HoldWithdrawalAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("withdrawals/{id:guid}/reject")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RejectWithdrawal(Guid id, [FromBody] AdminWithdrawalActionDto request)
        {
            var response = await _service.Admin.RejectWithdrawalAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }

        // ── KYC ────────────────────────────────────────────────────────────────

        [HttpGet("kyc/queue")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminKycQueueItemDto>>), 200)]
        public async Task<IActionResult> GetKycQueue([FromQuery] AdminKycQueryDto query)
        {
            var response = await _service.Admin.GetKycQueueAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("kyc/{submissionId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<AdminKycDetailDto>), 200)]
        public async Task<IActionResult> GetKycDetail(Guid submissionId)
        {
            var response = await _service.Admin.GetKycDetailAsync(submissionId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("kyc/{submissionId:guid}/approve")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ApproveKyc(Guid submissionId)
        {
            var response = await _service.Admin.ApproveKycAsync(submissionId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("kyc/{submissionId:guid}/reject")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RejectKyc(Guid submissionId, [FromBody] AdminKycActionDto request)
        {
            var response = await _service.Admin.RejectKycAsync(submissionId, request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
