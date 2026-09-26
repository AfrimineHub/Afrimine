using Afrimine.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{
    public class BuyerDashboardSummaryDto
    {
        public int SavedListingsCount { get; set; }
        public int UnreadMessagesCount { get; set; }
        public int OngoingOrdersCount { get; set; }
        public int OpenRfqsCount { get; set; }
    }

    public class MarketTrendDto
    {
        public string Commodity { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double ChangePercent { get; set; }
        public DateTime AsOf { get; set; }
    }

    public class InvestmentInsightDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class MarketplaceListingDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string CategoryLabel { get; set; } = string.Empty;
        public string? PrimaryImageUrl { get; set; }
        public decimal PriceAmount { get; set; }
        public string PriceCurrency { get; set; } = string.Empty;
        public string PriceUnit { get; set; } = string.Empty;
        public string? PriceDescription { get; set; }
        public string? MineralType { get; set; }
        public string? GradeOrPurity { get; set; }
        public string? EquipmentType { get; set; }
        public string? Condition { get; set; }
        public string? Quantity { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public int ViewsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MarketplaceListingDetailDto : MarketplaceListingDto
    {
        public string? ContactInfo { get; set; }
        public string? LeaseType { get; set; }
        public decimal? AcreageHectares { get; set; }
        public int? YearManufactured { get; set; }
        public string? ManpowerRole { get; set; }
        public string? Availability { get; set; }
        public List<ListingImageDto> Images { get; set; } = new();
    }

    public class MarketplaceQueryDto
    {
        public string? Q { get; set; }
        public string? Location { get; set; }
        public string? Mineral { get; set; }
        public ListingCategory? ListingType { get; set; }
        public bool VerifiedOnly { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CreateInquiryDto
    {
        [Required] public string Message { get; set; } = string.Empty;
    }

    public class BuyerOrderDto
    {
        public Guid Id { get; set; }
        public Guid ListingId { get; set; }
        public string ListingTitle { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BuyerOrderQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public OrderStatus? Status { get; set; }
    }

    public class DisputeOrderDto
    {
        [Required] public string Reason { get; set; } = string.Empty;
    }

    public class PayOrderDto
    {
        [Required] public string PaymentReference { get; set; } = string.Empty;
    }

    public class RfqDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MineralType { get; set; } = string.Empty;
        public string? Quantity { get; set; }
        public string? Unit { get; set; }
        public string? TargetPrice { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public int ResponseCount { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRfqDto
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public string MineralType { get; set; } = string.Empty;
        public string? Quantity { get; set; }
        public string? Unit { get; set; }
        public string? TargetPrice { get; set; }
        [Required] public string Location { get; set; } = string.Empty;
        [Required] public string Country { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);
    }

    public class RfqQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public RfqStatus? Status { get; set; }
    }
}
