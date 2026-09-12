using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IVendorListingService
    {
        Task<ApiResponse<PagedResultDto<VendorListingListDto>>> GetListingsAsync(string vendorId, VendorListingQueryDto query);
        Task<ApiResponse<VendorListingDetailDto>> GetListingByIdAsync(string vendorId, Guid listingId);
        Task<ApiResponse<VendorListingDetailDto>> CreateListingAsync(string vendorId, CreateListingDto request);
        Task<ApiResponse<VendorListingDetailDto>> UpdateListingAsync(string vendorId, Guid listingId, UpdateListingDto request);
        Task<ApiResponse<string>> DeleteListingAsync(string vendorId, Guid listingId);
        Task<ApiResponse<List<ListingImageDto>>> UploadImagesAsync(string vendorId, Guid listingId, UploadListingImagesDto request);
        Task<ApiResponse<string>> DeleteImageAsync(string vendorId, Guid listingId, Guid imageId);
        Task<ApiResponse<string>> PublishListingAsync(string vendorId, Guid listingId);
        Task<ApiResponse<RevenueSummaryDto>> GetRevenueSummaryAsync(string vendorId);
        Task<ApiResponse<PagedResultDto<VendorQuoteDto>>> GetQuotesAsync(string vendorId, VendorQuoteQueryDto query);
        Task<ApiResponse<PayoutSummaryDto>> GetPayoutSummaryAsync(string vendorId);
        Task<ApiResponse<PagedResultDto<VendorOrderDto>>> GetOrdersAsync(string vendorId, VendorOrderQueryDto query);
        Task<ApiResponse<PagedResultDto<ListingPerformanceItemDto>>> GetListingsPerformanceAsync(string vendorId, int page, int pageSize);
        Task<ApiResponse<VendorDashboardDto>> GetVendorDashboardAsync(string vendorId);
        Task<ApiResponse<string>> DisputeOrderAsync(string vendorId, Guid orderId, DisputeOrderDto request);
    }
}
