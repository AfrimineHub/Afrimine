using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Order : BaseEntity
    {
        public string BuyerId { get; set; } = string.Empty;
        public User Buyer { get; set; } = null!;
        public Guid ListingId { get; set; }
        public Listing Listing { get; set; } = null!;
        public string VendorId { get; set; } = string.Empty;
        public User Vendor { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? DisputeReason { get; set; }
        public string? PaymentReference { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
