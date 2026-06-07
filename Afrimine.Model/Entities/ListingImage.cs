namespace Afrimine.Model.Entities
{
    public class ListingImage : BaseEntity
    {
        public Guid ListingId { get; set; }
        public Listing Listing { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }
}
