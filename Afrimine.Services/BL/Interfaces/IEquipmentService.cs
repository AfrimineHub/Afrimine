using Afrimine.Model.Enums;
using Afrimine.Model.ViewModels;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using static Afrimine.Services.DTOs.SupplierDto;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IEquipmentService
    {
        // Supplier onboarding
        //Task<ApiResponse<string>> RegisterSupplierAsync(SupplierRegisterDto request);
        Task<ApiResponse<SupplierProfileResponseDto>> UpdateProfileAsync(string userId, SupplierProfileUpdateDto request);
        Task<ApiResponse<string>> UpdateLocationAsync(string userId, SupplierLocationDto request);
        Task<ApiResponse<string>> UploadDocumentAsync(string userId, SupplierDocumentUploadDto request);
        Task<ApiResponse<string>> SubmitForVerificationAsync(string userId);
        Task<ApiResponse<SupplierStatusDto>> GetStatusAsync(string userId);
        Task<ApiResponse<SupplierProfileResponseDto>> GetProfileAsync(string userId);

        // Assets
        Task<ApiResponse<AssetResponseDto>> CreateAssetAsync(Guid supplierId, CreateAssetDto request);
        Task<ApiResponse<IEnumerable<AssetResponseDto>>> GetAssetsAsync(Guid supplierId);
        Task<ApiResponse<AssetResponseDto>> GetAssetAsync(Guid supplierId,string role, Guid assetId);
        Task<ApiResponse<AssetResponseDto>> UpdateAssetAsync(Guid supplierId, Guid assetId, UpdateAssetDto request);
        Task<ApiResponse<string>> DeleteAssetAsync(Guid supplierId, Guid assetId);
        Task<ApiResponse<AssetResponseDto>> UploadAssetPhotosAsync(Guid supplierId, Guid assetId, AssetPhotoUploadDto request);
        Task<ApiResponse<AssetPricingDto>> GetAssetPricingAsync(Guid assetId, int totalDays, double distanceKm, string currency = "NGN");

        // Operators
        Task<ApiResponse<OperatorResponseDto>> CreateOperatorAsync(Guid supplierId, CreateOperatorDto request);
        Task<ApiResponse<IEnumerable<OperatorResponseDto>>> GetOperatorsAsync(Guid supplierId);
        Task<ApiResponse<OperatorResponseDto>> UpdateOperatorAsync(Guid supplierId, Guid operatorId, CreateOperatorDto request);
        Task<ApiResponse<string>> AssignOperatorToAssetAsync(Guid supplierId, Guid assetId, Guid operatorId);
        Task<ApiResponse<string>> AddGuarantorAsync(Guid supplierId, Guid operatorId, CreateGuarantorDto request);
        Task<ApiResponse<string>> SubmitVettingAsync(Guid supplierId, Guid operatorId, VettingSubmitDto request);
        Task<ApiResponse<VettingStatusDto>> GetVettingStatusAsync(Guid operatorId);

        // Bookings
        Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(string minerId, CreateBookingDto request);
        Task<ApiResponse<IEnumerable<BookingResponseDto>>> GetBookingsAsync(string userId, string? status);
        Task<ApiResponse<BookingDetailDto>> GetBookingDetailAsync(string userId, Guid bookingId);
        Task<ApiResponse<BookingResponseDto>> ApproveBookingAsync(Guid supplierId, Guid bookingId);
        Task<ApiResponse<string>> DeclineBookingAsync(Guid supplierId, Guid bookingId, DeclineBookingDto request);

        // Logistics & milestones
        Task<ApiResponse<string>> DispatchBookingAsync(Guid supplierId, Guid bookingId);
        Task<ApiResponse<LogisticsStatusDto>> GetLogisticsStatusAsync(Guid bookingId);
        Task<ApiResponse<TrackingDto>> GetTrackingAsync(Guid bookingId);
        Task<ApiResponse<string>> TriggerInsuranceAsync(Guid supplierId, Guid bookingId, string type);
        Task<ApiResponse<string>> SiteArrivalSignOffAsync(Guid supplierId, Guid bookingId);
        Task<ApiResponse<string>> SubmitDailyCheckAsync(Guid supplierId, Guid bookingId, DailyCheckDto request);
        Task<ApiResponse<IEnumerable<MilestoneDto>>> GetMilestonesAsync(Guid bookingId);
        Task<ApiResponse<string>> ReturnClearanceAsync(string userId, Guid bookingId);
        Task<ApiResponse<PaymentBreakdownDto>> GetPaymentBreakdownAsync(Guid bookingId);

        // Disputes
        Task<ApiResponse<string>> RaiseDisputeAsync(string userId, Guid bookingId, BookingDisputeDto request);
        Task<ApiResponse<IEnumerable<BookingDisputeResponseDto>>> GetBookingDisputesAsync(Guid bookingId);
        Task<ApiResponse<IEnumerable<BookingDisputeResponseDto>>> GetAllSupplierDisputesAsync(Guid supplierId);

        // Wallet
        Task<ApiResponse<WalletBalanceDto>> GetWalletBalanceAsync(Guid supplierId);
        Task<ApiResponse<string>> RequestWithdrawalAsync(Guid supplierId, WithdrawalRequestDto request);
        Task<ApiResponse<IEnumerable<WalletTransactionDto>>> GetWalletTransactionsAsync(Guid supplierId);

        // Dashboard
        Task<ApiResponse<SupplierDashboardStatsDto>> GetDashboardStatsAsync(Guid supplierId);

        // PayScrow
        Task<ApiResponse<string>> ApplyEscrowCodeAsync(string userId, EscrowCodeApplyDto request);
        Task<ApiResponse<object>> GetPayscrowTransactionStatusAsync(string transactionNumber);
        Task<ApiResponse<object>> CalculatePayscrowChargesAsync(string currencyCode, decimal amount, decimal merchantChargePercentage = 0);
        Task<ApiResponse<IEnumerable<PayscrowBank>>> GetSupportedBanksAsync();

        // Webhook handler
        Task HandlePayscrowWebhookAsync(PayscrowWebhookPayload payload);
        Task<ApiResponse<PagedResultDto<AssetResponseDto>>> SearchAssetsAsync(string? q, MachineType? machineType, string? location, decimal? maxDailyRate, bool availableOnly, int page, int pageSize);
    }
}
