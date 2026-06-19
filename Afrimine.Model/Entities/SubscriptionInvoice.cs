using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class SubscriptionInvoice : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public string PlanName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "NGN";
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
        public DateTime? PaidAt { get; set; }
        public string? InvoiceUrl { get; set; }
        public string? PaymentReference { get; set; }
        public string? SessionId { get; set; }
    }
}
