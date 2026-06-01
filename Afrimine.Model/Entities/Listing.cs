using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Listing : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public ListingCategory Category { get; set; }
        public ListingStatus Status { get; set; } = ListingStatus.Active;
        public string OwnerId { get; set; } = string.Empty;
        public User Owner { get; set; } = null!;
        public ICollection<SavedListing> SavedByUsers { get; set; } = new List<SavedListing>();
    }
}
