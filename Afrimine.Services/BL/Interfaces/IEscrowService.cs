using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IEscrowService
    {
        Task<ApiResponse<EscrowStatusDto>> GetEscrowStatusAsync(string userId, Guid orderId);
        Task<ApiResponse<EscrowCheckoutResponseDto>> CheckoutEscrowAsync(string buyerId, Guid orderId);
        Task<ApiResponse<EscrowStatusDto>> VerifyEscrowPaymentAsync(string buyerId, Guid orderId, string sessionId);
        Task<ApiResponse<EscrowStatusDto>> GetVendorEscrowStatusAsync(string vendorId, Guid orderId);
        Task<ApiResponse<string>> MarkDeliveredAsync(string vendorId, Guid orderId);
        Task<ApiResponse<string>> SubmitRfqQuoteAsync(string vendorId, Guid rfqId, VendorQuoteSubmitDto request);
        Task<ApiResponse<string>> RequestPayoutAsync(string vendorId, PayoutRequestDto request);
        Task<ApiResponse<IEnumerable<RfqQuoteDto>>> GetRfqQuotesAsync(string buyerId, Guid rfqId);
        Task<ApiResponse<string>> AcceptRfqQuoteAsync(string buyerId, Guid rfqId, Guid quoteId);
        Task<ApiResponse<PagedResultDto<DisputeDto>>> GetDisputesAsync(int page, int pageSize, string? status);
        Task<ApiResponse<DisputeDto>> GetDisputeByIdAsync(Guid disputeId);
        Task<ApiResponse<string>> ResolveDisputeAsync(string adminId, Guid disputeId, ResolveDisputeDto request);
        Task<ApiResponse<string>> FreezeOrderAsync(string adminId, Guid orderId);
        Task<ApiResponse<string>> ReleaseFundsAsync(string adminId, Guid orderId);
    }
}
