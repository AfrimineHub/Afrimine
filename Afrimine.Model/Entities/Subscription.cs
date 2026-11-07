namespace Afrimine.Model.Entities
{
    public class Subscription : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public string PlanId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public int ListingsLimit { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime RenewsAt { get; set; } = DateTime.UtcNow;
    }
}
