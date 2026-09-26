using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepositoryManager _repository;

        public DashboardService(IRepositoryManager repository)
        {
            _repository = repository;
        }

        //public async Task<ApiResponse<DashboardSummaryDto>> GetSummaryAsync(string userId)
        //{
        //    var savedCount = await _repository.SavedListing.CountByUserIdAsync(userId);
        //    var ongoingOrders = await _repository.Order.CountByUserAndStatusAsync(userId, OrderStatus.Ongoing);
        //    var unreadNotifications = await _repository.Notification.CountUnreadAsync(userId);

        //    return ApiResponse<DashboardSummaryDto>.Ok(new DashboardSummaryDto
        //    {
        //        SavedListingsCount = savedCount,
        //        UnreadMessagesCount = unreadNotifications,
        //        OngoingOrdersCount = ongoingOrders
        //    });
        //}

        public async Task<ApiResponse<DashboardSummaryDto>> GetSummaryAsync(string userId)
        {
            var savedCount = await _repository.SavedListing.CountByUserIdAsync(userId);
            var ongoingOrders = await _repository.Order.CountByUserAndStatusAsync(userId, OrderStatus.Ongoing);
            var unreadNotifications = await _repository.Notification.CountUnreadAsync(userId);
            var totalListings = await _repository.Listing.CountByVendorAsync(userId);
            var activeQuotes = await _repository.Listing.CountActiveQuotesAsync(userId);
            var successfulOrders = await _repository.Order.CountSuccessfulOrdersAsync(userId);
            var (payoutAmount, payoutCurrency) = await _repository.Order.GetPendingPayoutAsync(userId);

            return ApiResponse<DashboardSummaryDto>.Ok(new DashboardSummaryDto
            {
                SavedListingsCount = savedCount,
                UnreadMessagesCount = unreadNotifications,
                OngoingOrdersCount = ongoingOrders,
                TotalListingsCount = totalListings,
                ActiveQuotesCount = activeQuotes,
                SuccessfulOrdersCount = successfulOrders,
                PendingPayoutAmount = payoutAmount,
                PendingPayoutCurrency = payoutCurrency
            });
        }

        public async Task<ApiResponse<IEnumerable<ListingCardDto>>> GetRecommendedListingsAsync(string userId)
        {
            var listings = await _repository.Listing.GetRecommendedAsync(userId);
            return ApiResponse<IEnumerable<ListingCardDto>>.Ok(listings.Select(MapToListingCard));
        }

        public async Task<ApiResponse<IEnumerable<NotificationDto>>> GetLatestNotificationsAsync(string userId)
        {
            var notifications = await _repository.Notification.GetByUserIdAsync(userId);
            return ApiResponse<IEnumerable<NotificationDto>>.Ok(notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }));
        }

        public async Task<ApiResponse<IEnumerable<SavedListingDto>>> GetSavedListingsAsync(string userId, int page = 1, int pageSize = 10)
        {
            var saved = await _repository.SavedListing.GetByUserIdAsync(userId, page, pageSize);
            return ApiResponse<IEnumerable<SavedListingDto>>.Ok(saved.Select(s => new SavedListingDto
            {
                SavedId = s.Id,
                Listing = MapToListingCard(s.Listing),
                SavedAt = s.CreatedAt
            }));
        }

        public async Task<ApiResponse<string>> SaveListingAsync(string userId, SaveListingRequestDto request)
        {
            var listing = await _repository.Listing.GetByIdAsync(request.ListingId);
            if (listing is null)
                return ApiResponse<string>.Fail("Listing not found.", 404);

            var existing = await _repository.SavedListing.GetByUserAndListingAsync(userId, request.ListingId);
            if (existing is not null)
                return ApiResponse<string>.Fail("Listing already saved.", 409);

            await _repository.SavedListing.Create(new SavedListing
            {
                UserId = userId,
                ListingId = request.ListingId
            });
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Listing saved successfully.");
        }

        public async Task<ApiResponse<string>> UnsaveListingAsync(string userId, Guid listingId)
        {
            var saved = await _repository.SavedListing.GetByUserAndListingAsync(userId, listingId);
            if (saved is null)
                return ApiResponse<string>.Fail("Saved listing not found.", 404);

            _repository.SavedListing.Delete(saved);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Listing removed from saved.");
        }

        public async Task<ApiResponse<string>> MarkNotificationsReadAsync(string userId)
        {
            await _repository.Notification.MarkAllAsReadAsync(userId);
            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Notifications marked as read.");
        }

        public async Task<ApiResponse<SubscriptionSummaryDto>> GetSubscriptionAsync(string userId)
        {
            var subscription = await _repository.Subscription.GetActiveByUserIdAsync(userId);

            if (subscription is null)
                return ApiResponse<SubscriptionSummaryDto>.Fail("No active subscription found.", 404);

            var listingsUsed = await _repository.Listing.CountByVendorAsync(userId);
            var listingsRemaining = Math.Max(0, subscription.ListingsLimit - listingsUsed);
            var usagePercent = subscription.ListingsLimit > 0
                ? Math.Round((double)listingsUsed / subscription.ListingsLimit * 100, 1)
                : 0;

            // CanUpgrade = true when usage is at or above 80% or limit is reached
            var canUpgrade = usagePercent >= 80;

            return ApiResponse<SubscriptionSummaryDto>.Ok(new SubscriptionSummaryDto
            {
                PlanId = subscription.PlanId,
                PlanName = subscription.PlanName,
                ListingsLimit = subscription.ListingsLimit,
                ListingsUsed = listingsUsed,
                ListingsRemaining = listingsRemaining,
                UsagePercent = usagePercent,
                CanUpgrade = canUpgrade,
                RenewsAt = subscription.RenewsAt
            });
        }

        private static ListingCardDto MapToListingCard(Listing listing) => new()
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            Location = listing.Location,
            Country = listing.Country,
            ImageUrl = listing.ImageUrl,
            Category = listing.CategoryType.ToString(),
            CreatedAt = listing.CreatedAt
        };
    }
}
