using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse<AdminDashboardDto>> GetDashboardAsync();

        Task<ApiResponse<PagedResultDto<AdminUserListItemDto>>> GetUsersAsync(AdminUserQueryDto query);
        Task<ApiResponse<AdminUserStatsDto>> GetUserStatsAsync();
        Task<ApiResponse<string>> SuspendUserAsync(string userId, AdminUserActionDto request);
        Task<ApiResponse<string>> BanUserAsync(string userId, AdminUserActionDto request);
        Task<ApiResponse<string>> ReactivateUserAsync(string userId);

        Task<ApiResponse<PagedResultDto<AdminListingListItemDto>>> GetListingsAsync(AdminListingQueryDto query);
        Task<ApiResponse<AdminListingCountsDto>> GetListingCountsAsync();
        Task<ApiResponse<string>> ApproveListingAsync(Guid listingId);
        Task<ApiResponse<string>> RejectListingAsync(Guid listingId, AdminListingActionDto request);
        Task<ApiResponse<string>> FlagListingAsync(Guid listingId, AdminListingActionDto request);
        Task<ApiResponse<string>> DeleteListingAsync(Guid listingId);

        Task<ApiResponse<PagedResultDto<AdminQuoteListItemDto>>> GetQuotesAsync(AdminQuoteQueryDto query);

        Task<ApiResponse<PagedResultDto<AdminOrderListItemDto>>> GetOrdersAsync(AdminOrderQueryDto query);
        Task<ApiResponse<AdminOrderSummaryDto>> GetOrderSummaryAsync(string? q);
        Task<ApiResponse<AdminOrderDetailDto>> GetOrderDetailAsync(Guid orderId);

        Task<ApiResponse<AdminRevenueSummaryDto>> GetRevenueSummaryAsync();
        Task<ApiResponse<PagedResultDto<AdminTransactionItemDto>>> GetTransactionsAsync(AdminTransactionQueryDto query);

        Task<ApiResponse<PagedResultDto<AdminWithdrawalItemDto>>> GetWithdrawalsAsync(AdminWithdrawalQueryDto query);
        Task<ApiResponse<string>> ApproveWithdrawalAsync(Guid payoutId);
        Task<ApiResponse<string>> HoldWithdrawalAsync(Guid payoutId, AdminWithdrawalActionDto request);
        Task<ApiResponse<string>> RejectWithdrawalAsync(Guid payoutId, AdminWithdrawalActionDto request);

        Task<ApiResponse<PagedResultDto<AdminKycQueueItemDto>>> GetKycQueueAsync(AdminKycQueryDto query);
        Task<ApiResponse<AdminKycDetailDto>> GetKycDetailAsync(Guid profileId);
        Task<ApiResponse<string>> ApproveKycAsync(Guid profileId);
        Task<ApiResponse<string>> RejectKycAsync(Guid profileId, AdminKycActionDto request);
        Task<ApiResponse<string>> CreateAdminAsync(CreateAdminDto request);

        Task<ApiResponse<AdminUserListItemDto>> CreateUserAsync(AdminCreateUserDto request);
        Task<ApiResponse<AdminUserListItemDto>> UpdateUserAsync(string userId, AdminUpdateUserDto request);
        Task<ApiResponse<string>> DeleteUserAsync(string userId);
    }
}
