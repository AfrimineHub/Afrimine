using Afrimine.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afrimine.Model.Entities
{
    public class SupplierProfile : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public string? CompanyName { get; set; }
        public string? BusinessPhone { get; set; }
        public string? BusinessEmail { get; set; }
        public string? PrimaryBaseCity { get; set; }
        public string? YardAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? CacCertificateUrl { get; set; }
        public string? CacCertificatePublicId { get; set; }
        public SupplierStatus Status { get; set; } = SupplierStatus.Pending;
        public string? RejectionReason { get; set; }
        public int OnboardingStep { get; set; } = 1;
        public bool IsSubmitted { get; set; } = false;
        public string? BankName { get; set; }
        public string? BankCode { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        //kyc fields from vendor model
        public string? OfficeAddress { get; set; }
        public string? DateOfBirth { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? DocumentIdNumber { get; set; }
        public string? DocumentUrl { get; set; }
        public string? DocumentFileName { get; set; }
        public string? DocumentPublicId { get; set; }
        public long? DocumentFileSizeBytes { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? Country { get; set; }

        // ── NEW: fields from VendorProfile ──
        public BusinessType BusinessType { get; set; }
        public string StateOrRegion { get; set; } = string.Empty;
        public string? Website { get; set; }
        public VendorType VendorType { get; set; } = VendorType.EquipmentSupplier;
        public KycStatus KycStatus { get; set; } = KycStatus.NotStarted;
        public string? KycRejectionReason { get; set; }
        public bool IsComplete { get; set; } = false;
    }
}
