using Afrimine.Model.Enums;

namespace Afrimine.Services.DTOs
{
    public class VendorQuoteDto
    {
        public Guid Id { get; set; }
        public Guid ListingId { get; set; }
        public string ListingTitle { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class VendorQuoteQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public QuoteStatus? Status { get; set; }
    }

    public class PayoutSummaryDto
    {
        public string Currency { get; set; } = "USD";
        public decimal PendingAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public IEnumerable<PayoutItemDto> RecentPayouts { get; set; } = Enumerable.Empty<PayoutItemDto>();
    }

    public class PayoutItemDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class VendorOrderDto
    {
        public Guid Id { get; set; }
        public Guid ListingId { get; set; }
        public string ListingTitle { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class VendorOrderQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public OrderStatus? Status { get; set; }
    }

    public class ListingPerformanceItemDto
    {
        public Guid ListingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ViewsCount { get; set; }
        public int SavesCount { get; set; }
        public int InquiriesCount { get; set; }
    }

    public class VendorDashboardDto
    {
        public SubscriptionSummaryDto? Subscription { get; set; }
        public RevenueSummaryDto? Revenue { get; set; }
        public VendorDashboardStatsDto Stats { get; set; } = new();
        public IEnumerable<ListingPerformanceItemDto> ListingPerformance { get; set; } = Enumerable.Empty<ListingPerformanceItemDto>();
        public IEnumerable<NotificationDto> RecentNotifications { get; set; } = Enumerable.Empty<NotificationDto>();
    }

    public class VendorDashboardStatsDto
    {
        public int TotalListingsCount { get; set; }
        public int ActiveQuotesCount { get; set; }
        public int OngoingOrdersCount { get; set; }
        public int UnreadMessagesCount { get; set; }
        public decimal PendingPayoutAmount { get; set; }
        public int SuccessfulOrdersCount { get; set; }
    }
}
