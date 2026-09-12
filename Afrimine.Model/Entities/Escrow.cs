using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Escrow : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public string BuyerId { get; set; } = string.Empty;
        public string VendorId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "NGN";
        public EscrowStatus Status { get; set; } = EscrowStatus.Pending;
        public string? PaymentReference { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? SessionId { get; set; }
        public DateTime? FundedAt { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public DateTime? FrozenAt { get; set; }
    }
}
