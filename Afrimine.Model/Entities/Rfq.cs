using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Rfq : BaseEntity
    {
        public string BuyerId { get; set; } = string.Empty;
        public User Buyer { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MineralType { get; set; } = string.Empty;
        public string? Quantity { get; set; }
        public string? Unit { get; set; }
        public string? TargetPrice { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public RfqStatus Status { get; set; } = RfqStatus.Open;
        public DateTime ExpiresAt { get; set; }
    }
}
