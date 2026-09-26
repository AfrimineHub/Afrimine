namespace Afrimine.Model.Entities
{
    public class Inquiry : BaseEntity
    {
        public string BuyerId { get; set; } = string.Empty;
        public User Buyer { get; set; } = null!;
        public string VendorId { get; set; } = string.Empty;
        public User Vendor { get; set; } = null!;
        public Guid ListingId { get; set; }
        public Listing Listing { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
    }
}
