using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Listing : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public ListingCategory CategoryType { get; set; }
        public ListingStatus Status { get; set; } = ListingStatus.Active;
        public string OwnerId { get; set; } = string.Empty;
        public User Owner { get; set; } = null!;
        public string? PriceDescription { get; set; }
        public string? ContactInfo { get; set; }

        public string StateOrRegion { get; set; } = string.Empty;

        public decimal PriceAmount { get; set; }

        public string PriceCurrency { get; set; } = string.Empty;

        public string PriceUnit { get; set; } = string.Empty;

        public string? Quantity { get; set; }

        public string? MineralType { get; set; }

        public string? GradeOrPurity { get; set; }

        public string? EquipmentType { get; set; }

        public int? YearManufactured { get; set; }

        public string? Condition { get; set; }

        public decimal? AcreageHectares { get; set; }

        public string? LeaseType { get; set; }

        public string? ManpowerRole { get; set; }

        public string? Availability { get; set; }

        public bool? Publish { get; set; }

        public DateTime? PublishedAt { get; set; } = DateTime.UtcNow;
        public string? AdminReviewNote { get; set; }
        public int ViewsCount { get; set; } = 0;
        public int InquiriesCount { get; set; } = 0;

        // Navigation
        public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();
        public ICollection<SavedListing> SavedByUsers { get; set; } = new List<SavedListing>();
    }
}
