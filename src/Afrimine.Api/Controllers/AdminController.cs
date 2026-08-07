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

        /// <summary>Get full admin dashboard — stats, alerts, activity, ongoing transactions</summary>
        /// <remarks>
        /// Single endpoint that powers the entire admin dashboard.
        ///
        /// **Stats included:** Total users, active users, KYC verified, vendors, total revenue, pending payments, open disputes, pending listings
        ///
        /// **Priority alerts:** Open disputes + pending KYC submissions needing urgent attention
        ///
        /// **Ongoing transactions:** Orders currently in `Ongoing` or `Paid` status
        /// </remarks>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<AdminDashboardDto>), 200)]
        public async Task<IActionResult> GetDashboard()
        {
            var response = await _service.Admin.GetDashboardAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>List all platform users with filtering</summary>
        /// <remarks>
        /// **role filter:** `vendor`, `buyer`, `investor`, `support`, `superadmin`
        /// **kycStatus filter:** `notstarted`, `pending`, `verified`, `rejected`
        /// **accountStatus filter:** `active`, `suspended`, `banned`, `pending`
        /// </remarks>

        [HttpGet("users")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminUserListItemDto>>), 200)]
        public async Task<IActionResult> GetUsers([FromQuery] AdminUserQueryDto query)
        {
            var response = await _service.Admin.GetUsersAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get user count stats for dashboard cards</summary>

        [HttpGet("users/stats")]
        [ProducesResponseType(typeof(ApiResponse<AdminUserStatsDto>), 200)]
        public async Task<IActionResult> GetUserStats()
        {
            var response = await _service.Admin.GetUserStatsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Suspend a user account</summary>
        /// <remarks>User loses access but account is preserved. Provide a reason for the suspension.</remarks>
        
        [HttpPost("users/{userId}/suspend")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SuspendUser(string userId, [FromBody] AdminUserActionDto request)
        {
            var response = await _service.Admin.SuspendUserAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>Permanently ban a user account</summary>
        /// <remarks>User is banned from the platform. Provide a reason.</remarks>
        
        [HttpPost("users/{userId}/ban")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> BanUser(string userId, [FromBody] AdminUserActionDto request)
        {
            var response = await _service.Admin.BanUserAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Reactivate a suspended or banned user</summary>

        [HttpPost("users/{userId}/reactivate")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ReactivateUser(string userId)
        {
            var response = await _service.Admin.ReactivateUserAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>List listings pending moderation</summary>
        /// <remarks>
        /// **status filter values:** `pendingreview`, `active`, `rejected`, `flagged`, `archived`
        /// Default shows all statuses if no filter provided.
        /// </remarks>
        
        [HttpGet("listings")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminListingListItemDto>>), 200)]
        public async Task<IActionResult> GetListings([FromQuery] AdminListingQueryDto query)
        {
            var response = await _service.Admin.GetListingsAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get listing counts by status for moderation tab badges</summary>
        [HttpGet("listings/counts")]
        [ProducesResponseType(typeof(ApiResponse<AdminListingCountsDto>), 200)]
        public async Task<IActionResult> GetListingCounts()
        {
            var response = await _service.Admin.GetListingCountsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Approve a listing — makes it live on the marketplace</summary>
        /// <remarks>Status changes from `PendingReview` to `Active`. Vendor is notified.</remarks>
        [HttpPost("listings/{id:guid}/approve")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ApproveListing(Guid id)
        {
            var response = await _service.Admin.ApproveListingAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Reject a listing with reason</summary>
        /// <remarks>Status changes to `Rejected`. Vendor sees the rejection reason and can edit and resubmit.</remarks>
        [HttpPost("listings/{id:guid}/reject")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RejectListing(Guid id, [FromBody] AdminListingActionDto request)
        {
            var response = await _service.Admin.RejectListingAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Flag a listing for further review</summary>
        /// <remarks>Status changes to `Flagged`. Listing remains visible but flagged for admin attention.</remarks>
        [HttpPost("listings/{id:guid}/flag")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> FlagListing(Guid id, [FromBody] AdminListingActionDto request)
        {
            var response = await _service.Admin.FlagListingAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Archive/permanently remove a listing</summary>
        /// <remarks>Soft deletes the listing — it is archived and no longer visible on the marketplace.</remarks>
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

        /// <summary>Get all orders with filtering</summary>
        /// <remarks>
        /// **status filter:** `pending`, `ongoing`, `paid`, `delivered`, `completed`, `disputed`, `frozen`, `cancelled`
        /// Use `q` to search by buyer name, vendor name, or listing title.
        /// </remarks>
        [HttpGet("orders")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminOrderListItemDto>>), 200)]
        public async Task<IActionResult> GetOrders([FromQuery] AdminOrderQueryDto query)
        {
            var response = await _service.Admin.GetOrdersAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get order count summary for status tabs</summary>
        [HttpGet("orders/summary")]
        [ProducesResponseType(typeof(ApiResponse<AdminOrderSummaryDto>), 200)]
        public async Task<IActionResult> GetOrderSummary([FromQuery] string? q)
        {
            var response = await _service.Admin.GetOrderSummaryAsync(q);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get full order detail with timeline and documents</summary>
        /// <remarks>
        /// Timeline shows the progression: Order Placed → Payment → In Escrow → Delivered → Completed
        /// Each step has a status of `completed`, `current`, or `pending`.
        /// </remarks>
        [HttpGet("orders/{orderId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<AdminOrderDetailDto>), 200)]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            var response = await _service.Admin.GetOrderDetailAsync(orderId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get platform revenue summary</summary>
        /// <remarks>
        /// Returns total revenue, vendor payouts, and pending payments with month-over-month change percentages.
        /// Change percent is null if there is no previous month data to compare against.
        /// </remarks>

        [HttpGet("revenue")]
        [ProducesResponseType(typeof(ApiResponse<AdminRevenueSummaryDto>), 200)]
        public async Task<IActionResult> GetRevenue()
        {
            var response = await _service.Admin.GetRevenueSummaryAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get all revenue transactions (paginated)</summary>
        /// <remarks>**status filter:** `completed`, `pending`, `failed`</remarks>
        [HttpGet("revenue/transactions")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminTransactionItemDto>>), 200)]
        public async Task<IActionResult> GetTransactions([FromQuery] AdminTransactionQueryDto query)
        {
            var response = await _service.Admin.GetTransactionsAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>List vendor withdrawal requests</summary>
        /// <remarks>
        /// **status filter values:** `pending`, `processing`, `completed`, `failed`
        /// Use `q` to search by vendor name or email.
        /// </remarks>
        [HttpGet("withdrawals")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminWithdrawalItemDto>>), 200)]
        public async Task<IActionResult> GetWithdrawals([FromQuery] AdminWithdrawalQueryDto query)
        {
            var response = await _service.Admin.GetWithdrawalsAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Approve a withdrawal and release funds to vendor's bank</summary>
        [HttpPost("withdrawals/{id:guid}/approve")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ApproveWithdrawal(Guid id)
        {
            var response = await _service.Admin.ApproveWithdrawalAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Place a withdrawal on hold pending review</summary>
        /// <remarks>Provide a reason that will be visible to the vendor.</remarks>
        [HttpPost("withdrawals/{id:guid}/hold")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> HoldWithdrawal(Guid id, [FromBody] AdminWithdrawalActionDto request)
        {
            var response = await _service.Admin.HoldWithdrawalAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Reject a withdrawal request</summary>
        /// <remarks>Funds return to vendor's available balance. Provide a reason.</remarks>
        [HttpPost("withdrawals/{id:guid}/reject")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RejectWithdrawal(Guid id, [FromBody] AdminWithdrawalActionDto request)
        {
            var response = await _service.Admin.RejectWithdrawalAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get KYC verification queue</summary>
        /// <remarks>
        /// **status filter:** `pending` (default), `verified`, `rejected`, `notstarted`
        /// Use `q` to search by vendor name or email.
        /// </remarks>
        [HttpGet("kyc/queue")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminKycQueueItemDto>>), 200)]
        public async Task<IActionResult> GetKycQueue([FromQuery] AdminKycQueryDto query)
        {
            var response = await _service.Admin.GetKycQueueAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get full KYC submission details including document download URL</summary>
        /// <remarks>
        /// Returns all vendor identity information and the URL to download/view their uploaded document.
        /// Use `documentDownloadUrl` to view the document before approving or rejecting.
        /// </remarks>
        [HttpGet("kyc/{submissionId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<AdminKycDetailDto>), 200)]
        public async Task<IActionResult> GetKycDetail(Guid submissionId)
        {
            var response = await _service.Admin.GetKycDetailAsync(submissionId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Approve KYC — grants vendor full platform access</summary>
        /// <remarks>KYC status changes to `Verified`. Vendor is notified and can now publish listings.</remarks>
        [HttpPost("kyc/{submissionId:guid}/approve")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ApproveKyc(Guid submissionId)
        {
            var response = await _service.Admin.ApproveKycAsync(submissionId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Reject KYC with reason</summary>
        /// <remarks>
        /// KYC status changes to `Rejected`. Vendor sees the rejection reason and can resubmit.
        /// Provide a clear reason explaining what was wrong with the document.
        /// </remarks>
        [HttpPost("kyc/{submissionId:guid}/reject")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RejectKyc(Guid submissionId, [FromBody] AdminKycActionDto request)
        {
            var response = await _service.Admin.RejectKycAsync(submissionId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Create first admin account — one-time setup, remove after use</summary>
        /// <remarks>
        /// ⚠️ Remove this endpoint immediately after creating your first admin.
        /// Requires setup key in query param: `?setupKey= AFRIMINE_SETUP_2026`
        /// </remarks>
        //[AllowAnonymous]
        //[HttpPost("setup-admin")]
        //public async Task<IActionResult> SetupAdmin([FromBody] CreateAdminDto request,
        //    [FromQuery] string setupKey)
        //{
        //    if (setupKey != "AFRIMINE_SETUP_2026") return Unauthorized();
        //    var response = await _service.Admin.CreateAdminAsync(request);
        //    return StatusCode(response.StatusCode, response);
        //}

        /// <summary>Create a new user account (SuperAdmin only)</summary>
        /// <remarks>
        /// Admin creates a user account directly — email is auto-confirmed, no OTP required.
        ///
        /// **Role values:**
        /// - `1` = Vendor
        /// - `2` = Buyer
        /// - `3` = Investor
        /// - `4` = Support
        /// - `5` = SuperAdmin
        ///
        /// **VendorType (only when Role = 1 / Vendor):**
        /// - `0` = EquipmentSupplier
        /// - `1` = MineralSupplier
        /// - `2` = ManpowerSupplier
        ///
        /// A VendorProfile is auto-created when Role is Vendor.
        /// </remarks>
        [HttpPost("users")]
        [ProducesResponseType(typeof(ApiResponse<AdminUserListItemDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 409)]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto request)
        {
            var response = await _service.Admin.CreateUserAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Update a user's details (SuperAdmin only)</summary>
        /// <remarks>
        /// Partial update — only send the fields you want to change.
        /// Handles email change, role change, and account status update in one call.
        ///
        /// **AccountStatus values:**
        /// - `0` = Pending
        /// - `1` = Active
        /// - `2` = Suspended
        /// - `3` = Banned
        /// - `4` = Deactivated
        /// </remarks>
        [HttpPut("users/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<AdminUserListItemDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 409)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] AdminUpdateUserDto request)
        {
            var response = await _service.Admin.UpdateUserAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Permanently delete a user account (SuperAdmin only)</summary>
        /// <remarks>
        /// Permanently deletes the user and all associated Identity records.
        /// Cannot delete SuperAdmin accounts.
        ///
        /// **⚠️ This action is irreversible.**
        /// For temporary restrictions, use `POST /admin/users/{userId}/suspend` or `POST /admin/users/{userId}/ban` instead.
        /// </remarks>
        [HttpDelete("users/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 403)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var response = await _service.Admin.DeleteUserAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
