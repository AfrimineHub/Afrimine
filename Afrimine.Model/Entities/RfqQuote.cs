using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class RfqQuote : BaseEntity
    {
        public Guid RfqId { get; set; }
        public Rfq Rfq { get; set; } = null!;
        public string VendorId { get; set; } = string.Empty;
        public User Vendor { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? Note { get; set; }
        public QuoteStatus Status { get; set; } = QuoteStatus.Pending;
    }
}
