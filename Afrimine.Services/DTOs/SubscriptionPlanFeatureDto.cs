using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{
    public class SubscriptionPlanFeatureDto
    {
        public string Text { get; set; } = string.Empty;
        public bool Included { get; set; }
    }

    public class SubscriptionPlanDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PriceMonthly { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int ListingsLimit { get; set; }
        public bool IsPopular { get; set; }
        public List<SubscriptionPlanFeatureDto> Features { get; set; } = new();
    }

    public class CheckoutRequestDto
    {
        [Required] public string PlanId { get; set; } = string.Empty;
        [Required] public string SuccessUrl { get; set; } = string.Empty;
        [Required] public string CancelUrl { get; set; } = string.Empty;
    }

    public class CheckoutResponseDto
    {
        public string CheckoutUrl { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string Provider { get; set; } = "paystack";
    }

    public class ChangePlanDto
    {
        public string? Reason { get; set; }
    }

    public class CancelSubscriptionDto
    {
        public string? Reason { get; set; }
    }

    public class ContactSalesDto
    {
        [Required] public string Message { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        public string? PreferredPlanId { get; set; }
    }

    public class InvoiceDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? InvoiceUrl { get; set; }
    }
}
