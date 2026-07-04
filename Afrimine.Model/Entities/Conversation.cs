namespace Afrimine.Model.Entities
{
    public class Conversation : BaseEntity
    {
        public string BuyerId { get; set; } = string.Empty;
        public User Buyer { get; set; } = null!;
        public string VendorId { get; set; } = string.Empty;
        public User Vendor { get; set; } = null!;
        public Guid? ListingId { get; set; }
        public Listing? Listing { get; set; }
        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public Guid? RfqId { get; set; }
        public Rfq? Rfq { get; set; }
    }
}
