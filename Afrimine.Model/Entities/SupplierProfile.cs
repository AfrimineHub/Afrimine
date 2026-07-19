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
    }
}
