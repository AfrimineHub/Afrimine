namespace Afrimine.Services.DTOs
{
    public class DashboardSummaryDto
    {
        public int SavedListingsCount { get; set; }
        public int UnreadMessagesCount { get; set; }
        public int OngoingOrdersCount { get; set; }
        public int TotalListingsCount { get; set; }
        public int ActiveQuotesCount { get; set; }
        public decimal PendingPayoutAmount { get; set; }
        public string PendingPayoutCurrency { get; set; } = "USD";
        public int SuccessfulOrdersCount { get; set; }
        public int OpenRfqsCount { get; set; }
    }

    public class ListingCardDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class SavedListingDto
    {
        public Guid SavedId { get; set; }
        public ListingCardDto Listing { get; set; } = null!;
        public DateTime SavedAt { get; set; }
    }

    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SaveListingRequestDto
    {
        public Guid ListingId { get; set; }
    }

}
