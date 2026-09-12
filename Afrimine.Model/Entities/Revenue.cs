namespace Afrimine.Model.Entities
{
    public class Revenue : BaseEntity
    {
        public string VendorId { get; set; } = string.Empty;
        public User Vendor { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public Guid? ListingId { get; set; }
        public Listing? Listing { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
