using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Shared.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Afrimine.Services.DTOs.SupplierDto;

namespace Afrimine.Api.Controllers.V1
{
    [Route("api/v{version:apiversion}")]
    [ApiVersion("1.0")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IServiceManager _service;

        public EquipmentController(IServiceManager service)
        {
            _service = service;
        }

        // ── Auth / Onboarding ──────────────────────────────────────────────────

        [AllowAnonymous]
        [HttpPost("suppliers/register")]
        public async Task<IActionResult> Register([FromBody] SupplierRegisterDto request)
        {
            var response = await _service.Equipment.RegisterSupplierAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("suppliers/me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetProfileAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPatch("suppliers/profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] SupplierProfileUpdateDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UpdateProfileAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPut("suppliers/location")]
        public async Task<IActionResult> UpdateLocation([FromBody] SupplierLocationDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UpdateLocationAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("suppliers/documents")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument([FromForm] SupplierDocumentUploadDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UploadDocumentAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("suppliers/submit")]
        public async Task<IActionResult> Submit()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.SubmitForVerificationAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("suppliers/status")]
        public async Task<IActionResult> GetStatus()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetStatusAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Assets ─────────────────────────────────────────────────────────────

        [Authorize]
        [HttpPost("assets")]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.CreateAssetAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("assets")]
        public async Task<IActionResult> GetAssets()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetAssetsAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("assets/{assetId:guid}")]
        public async Task<IActionResult> GetAsset(Guid assetId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetAssetAsync(userId, assetId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPut("assets/{assetId:guid}")]
        public async Task<IActionResult> UpdateAsset(Guid assetId, [FromBody] UpdateAssetDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UpdateAssetAsync(userId, assetId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpDelete("assets/{assetId:guid}")]
        public async Task<IActionResult> DeleteAsset(Guid assetId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.DeleteAssetAsync(userId, assetId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("assets/{assetId:guid}/photos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAssetPhotos(Guid assetId, [FromForm] AssetPhotoUploadDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UploadAssetPhotosAsync(userId, assetId, request);
            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpGet("assets/{assetId:guid}/pricing")]
        public async Task<IActionResult> GetPricing(
            Guid assetId, [FromQuery] int totalDays, [FromQuery] double distanceKm,
            [FromQuery] string currency = "NGN")
        {
            var response = await _service.Equipment.GetAssetPricingAsync(assetId, totalDays, distanceKm, currency);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("assets/{assetId:guid}/operators")]
        public async Task<IActionResult> AssignOperator(Guid assetId, [FromQuery] Guid operatorId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.AssignOperatorToAssetAsync(userId, assetId, operatorId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Operators ──────────────────────────────────────────────────────────

        [Authorize]
        [HttpPost("operators")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateOperator([FromForm] CreateOperatorDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.CreateOperatorAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("operators")]
        public async Task<IActionResult> GetOperators()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetOperatorsAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPut("operators/{operatorId:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateOperator(Guid operatorId, [FromForm] CreateOperatorDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.UpdateOperatorAsync(userId, operatorId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("operators/{operatorId:guid}/guarantors")]
        public async Task<IActionResult> AddGuarantor(Guid operatorId, [FromBody] CreateGuarantorDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.AddGuarantorAsync(userId, operatorId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("operators/{operatorId:guid}/vetting")]
        public async Task<IActionResult> SubmitVetting(Guid operatorId, [FromBody] VettingSubmitDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.SubmitVettingAsync(userId, operatorId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("operators/{operatorId:guid}/vetting-status")]
        public async Task<IActionResult> GetVettingStatus(Guid operatorId)
        {
            var response = await _service.Equipment.GetVettingStatusAsync(operatorId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Bookings ───────────────────────────────────────────────────────────

        [Authorize]
        [HttpPost("bookings")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.CreateBookingAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings([FromQuery] string? status)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetBookingsAsync(userId, status);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("bookings/{bookingId:guid}")]
        public async Task<IActionResult> GetBooking(Guid bookingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetBookingDetailAsync(userId, bookingId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPut("bookings/{bookingId:guid}/approve")]
        public async Task<IActionResult> ApproveBooking(Guid bookingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.ApproveBookingAsync(userId, bookingId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPut("bookings/{bookingId:guid}/decline")]
        public async Task<IActionResult> DeclineBooking(Guid bookingId, [FromBody] DeclineBookingDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.DeclineBookingAsync(userId, bookingId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("bookings/{bookingId:guid}/contract")]
        public IActionResult GetContract(Guid bookingId)
        {
            // TODO: Generate PDF contract / E-Waybill
            return Ok(new { message = "Contract generation coming soon.", bookingId });
        }

        // ── Logistics ──────────────────────────────────────────────────────────

        [Authorize]
        [HttpPost("bookings/{bookingId:guid}/dispatch")]
        public async Task<IActionResult> Dispatch(Guid bookingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.DispatchBookingAsync(userId, bookingId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("bookings/{bookingId:guid}/logistics-status")]
        public async Task<IActionResult> GetLogisticsStatus(Guid bookingId)
        {
            var response = await _service.Equipment.GetLogisticsStatusAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("bookings/{bookingId:guid}/tracking")]
        public async Task<IActionResult> GetTracking(Guid bookingId)
        {
            var response = await _service.Equipment.GetTrackingAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("bookings/{bookingId:guid}/insurance")]
        public async Task<IActionResult> TriggerInsurance(Guid bookingId, [FromBody] TriggerInsuranceDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.TriggerInsuranceAsync(userId, bookingId, request.Type);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("bookings/{bookingId:guid}/insurance-certificate")]
        public async Task<IActionResult> GetInsuranceCertificate(Guid bookingId)
        {
            var response = await _service.Equipment.GetLogisticsStatusAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Milestones & Sign-off ──────────────────────────────────────────────

        [Authorize]
        [HttpPost("bookings/{bookingId:guid}/site-arrival")]
        public async Task<IActionResult> SiteArrival(Guid bookingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.SiteArrivalSignOffAsync(userId, bookingId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("bookings/{bookingId:guid}/daily-check")]
        public async Task<IActionResult> DailyCheck(Guid bookingId, [FromBody] DailyCheckDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.SubmitDailyCheckAsync(userId, bookingId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("bookings/{bookingId:guid}/milestones")]
        public async Task<IActionResult> GetMilestones(Guid bookingId)
        {
            var response = await _service.Equipment.GetMilestonesAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("bookings/{bookingId:guid}/return-clearance")]
        public async Task<IActionResult> ReturnClearance(Guid bookingId)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.ReturnClearanceAsync(userId, bookingId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("bookings/{bookingId:guid}/payment-breakdown")]
        public async Task<IActionResult> GetPaymentBreakdown(Guid bookingId)
        {
            var response = await _service.Equipment.GetPaymentBreakdownAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Disputes ───────────────────────────────────────────────────────────

        [Authorize]
        [HttpPost("bookings/{bookingId:guid}/disputes")]
        public async Task<IActionResult> RaiseDispute(Guid bookingId, [FromBody] BookingDisputeDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.RaiseDisputeAsync(userId, bookingId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("bookings/{bookingId:guid}/disputes")]
        public async Task<IActionResult> GetBookingDisputes(Guid bookingId)
        {
            var response = await _service.Equipment.GetBookingDisputesAsync(bookingId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("disputes")]
        public async Task<IActionResult> GetAllDisputes()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetAllSupplierDisputesAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Wallet ─────────────────────────────────────────────────────────────

        [Authorize]
        [HttpGet("wallet/balance")]
        public async Task<IActionResult> GetWalletBalance()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetWalletBalanceAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("wallet/withdrawal")]
        public async Task<IActionResult> RequestWithdrawal([FromBody] WithdrawalRequestDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.RequestWithdrawalAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("wallet/transactions")]
        public async Task<IActionResult> GetWalletTransactions()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetWalletTransactionsAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        // ── Dashboard ──────────────────────────────────────────────────────────

        [Authorize]
        [HttpGet("dashboard/supplier/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.GetDashboardStatsAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        // ── PayScrow ───────────────────────────────────────────────────────────

        [Authorize]
        [HttpPost("escrow/apply-code")]
        public async Task<IActionResult> ApplyEscrowCode([FromBody] EscrowCodeApplyDto request)
        {
            var userId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var response = await _service.Equipment.ApplyEscrowCodeAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("escrow/status/{transactionNumber}")]
        public async Task<IActionResult> GetPayscrowStatus(string transactionNumber)
        {
            var response = await _service.Equipment.GetPayscrowTransactionStatusAsync(transactionNumber);
            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpGet("escrow/charges")]
        public async Task<IActionResult> CalculateCharges(
            [FromQuery] string currencyCode, [FromQuery] decimal amount,
            [FromQuery] decimal merchantChargePercentage = 0)
        {
            var response = await _service.Equipment.CalculatePayscrowChargesAsync(
                currencyCode, amount, merchantChargePercentage);
            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpGet("banks")]
        public async Task<IActionResult> GetSupportedBanks()
        {
            var response = await _service.Equipment.GetSupportedBanksAsync();
            return StatusCode(response.StatusCode, response);
        }
    }
}
