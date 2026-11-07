using Afrimine.Model.Enums;

namespace Afrimine.Services.DTOs
{
    public class VendorProfileResponseDto
    {
        public BusinessType BusinessType { get; set; }
        public string Country { get; set; } = string.Empty;
        public string StateOrRegion { get; set; } = string.Empty;
        public string OfficeAddress { get; set; } = string.Empty;
        public string? Website { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? DocumentFileName { get; set; }
        public string? DocumentUrl { get; set; }
        public int OnboardingStep { get; set; }
        public bool IsComplete { get; set; }
    }
}
