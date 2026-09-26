namespace Afrimine.Services.DTOs
{
    public class SubscriptionSummaryDto
    {
        public string PlanId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public int ListingsLimit { get; set; }
        public int ListingsUsed { get; set; }
        public int ListingsRemaining { get; set; }
        public double UsagePercent { get; set; }
        public bool CanUpgrade { get; set; }
        public DateTime RenewsAt { get; set; } = DateTime.UtcNow;
    }
}
