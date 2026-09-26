using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Payout : BaseEntity
    {
        public string VendorId { get; set; } = string.Empty;
        public User Vendor { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public PayoutStatus Status { get; set; } = PayoutStatus.Pending;
        public DateTime? ProcessedAt { get; set; } = DateTime.UtcNow;
        public string? Reference { get; set; }
    }
}
