using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IBuyerService
    {
        Task<ApiResponse<BuyerDashboardSummaryDto>> GetBuyerSummaryAsync(string buyerId);
        Task<ApiResponse<PagedResultDto<MarketplaceListingDto>>> SearchListingsAsync(MarketplaceQueryDto query);
        Task<ApiResponse<MarketplaceListingDetailDto>> GetListingDetailAsync(Guid listingId, string? viewerId);
        Task<ApiResponse<IEnumerable<string>>> GetCategoriesAsync();
        Task<ApiResponse<string>> InquireListingAsync(string buyerId, Guid listingId, CreateInquiryDto request);
        Task<ApiResponse<PagedResultDto<BuyerOrderDto>>> GetOrdersAsync(string buyerId, BuyerOrderQueryDto query);
        Task<ApiResponse<BuyerOrderDto>> GetOrderByIdAsync(string buyerId, Guid orderId);
        Task<ApiResponse<string>> ConfirmDeliveryAsync(string buyerId, Guid orderId);
        Task<ApiResponse<string>> DisputeOrderAsync(string buyerId, Guid orderId, DisputeOrderDto request);
        Task<ApiResponse<string>> PayOrderAsync(string buyerId, Guid orderId, PayOrderDto request);
        Task<ApiResponse<PagedResultDto<RfqDto>>> GetBuyerRfqsAsync(string buyerId, RfqQueryDto query);
        Task<ApiResponse<RfqDto>> GetRfqByIdAsync(Guid rfqId);
        Task<ApiResponse<RfqDto>> CreateRfqAsync(string buyerId, CreateRfqDto request);
    }
}
