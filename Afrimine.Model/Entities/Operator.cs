using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Operator : BaseEntity
    {
        public string SupplierId { get; set; } = string.Empty;
        public User Supplier { get; set; } = null!;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseCategory { get; set; } = "E";
        public int YearsOfExperience { get; set; }
        public string? LicenseDocumentUrl { get; set; }
        public string? LicenseDocumentPublicId { get; set; }
        public VettingStatus VettingStatus { get; set; } = VettingStatus.NotStarted;
        public string? VettingAnswers { get; set; } // JSON
        public bool PassedVetting { get; set; } = false;
        public ICollection<Guarantor> Guarantors { get; set; } = new List<Guarantor>();
        public ICollection<AssetOperator> AssignedAssets { get; set; } = new List<AssetOperator>();
    }
}
