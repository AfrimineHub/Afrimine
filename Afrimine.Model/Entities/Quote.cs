using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Quote : BaseEntity
    {
        public string VendorId { get; set; } = string.Empty;
        public User Vendor { get; set; } = null!;
        public string BuyerId { get; set; } = string.Empty;
        public User Buyer { get; set; } = null!;
        public Guid ListingId { get; set; }
        public Listing Listing { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? Note { get; set; }
        public QuoteStatus Status { get; set; } = QuoteStatus.Pending;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;
    }
}
