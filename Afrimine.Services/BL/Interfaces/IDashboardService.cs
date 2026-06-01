using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardSummaryDto>> GetSummaryAsync(string userId);
        Task<ApiResponse<IEnumerable<ListingCardDto>>> GetRecommendedListingsAsync(string userId);
        Task<ApiResponse<IEnumerable<NotificationDto>>> GetLatestNotificationsAsync(string userId);
        Task<ApiResponse<IEnumerable<SavedListingDto>>> GetSavedListingsAsync(string userId, int page = 1, int pageSize = 10);
        Task<ApiResponse<string>> SaveListingAsync(string userId, SaveListingRequestDto request);
        Task<ApiResponse<string>> UnsaveListingAsync(string userId, Guid listingId);
        Task<ApiResponse<string>> MarkNotificationsReadAsync(string userId);
    }
}
