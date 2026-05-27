using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class VendorProfile
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public BusinessType BusinessType { get; set; }
        public string Country { get; set; } = string.Empty;
        public string StateOrRegion { get; set; } = string.Empty;
        public string OfficeAddress { get; set; } = string.Empty;
        public string? Website { get; set; }

        public DocumentType? DocumentType { get; set; }
        public string? DocumentUrl { get; set; }
        public string? DocumentFileName { get; set; }

        public int OnboardingStep { get; set; } = 1;
        public bool IsComplete { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
