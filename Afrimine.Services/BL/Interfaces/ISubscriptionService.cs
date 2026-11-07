using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Interfaces
{
    public interface ISubscriptionService
    {
        Task<ApiResponse<IEnumerable<SubscriptionPlanDto>>> GetPlansAsync();
        Task<ApiResponse<CheckoutResponseDto>> CheckoutAsync(string userId, CheckoutRequestDto request);
        Task<ApiResponse<SubscriptionSummaryDto>> ChangePlanAsync(string userId, ChangePlanDto request);
        Task<ApiResponse<SubscriptionSummaryDto>> CancelAsync(string userId, CancelSubscriptionDto request);
        Task<ApiResponse<PagedResultDto<InvoiceDto>>> GetInvoicesAsync(string userId, int page, int pageSize);
        Task<ApiResponse<string>> ContactSalesAsync(string userId, ContactSalesDto request);
        Task<ApiResponse<SubscriptionSummaryDto>> VerifyCheckoutAsync(string userId, string sessionId);
    }
}
