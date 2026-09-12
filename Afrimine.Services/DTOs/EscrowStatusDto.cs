using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{
    public class EscrowStatusDto
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? PaymentReference { get; set; }
        public DateTime? FundedAt { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public DateTime? FrozenAt { get; set; }
    }

    public class EscrowCheckoutResponseDto
    {
        public string CheckoutUrl { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string Provider { get; set; } = "paystack";
    }

    public class DisputeDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string ListingTitle { get; set; } = string.Empty;
        public string RaisedByName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AdminNote { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ResolveDisputeDto
    {
        [Required] public string Resolution { get; set; } = string.Empty; // "buyer" or "vendor"
        public string? AdminNote { get; set; }
    }

    public class VendorQuoteSubmitDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? Note { get; set; }
    }

    public class PayoutRequestDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? BankAccountDetails { get; set; }
    }

    public class RfqQuoteDto
    {
        public Guid Id { get; set; }
        public Guid RfqId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
