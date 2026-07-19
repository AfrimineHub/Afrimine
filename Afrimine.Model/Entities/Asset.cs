using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Asset : BaseEntity
    {
        public string SupplierId { get; set; } = string.Empty;
        public User Supplier { get; set; } = null!;
        public MachineType MachineType { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int YearOfManufacture { get; set; }
        public int EngineHours { get; set; }
        public bool HasCertifiedOperator { get; set; } = false;
        public AssetStatus Status { get; set; } = AssetStatus.Available;
        public decimal DailyRentalRate { get; set; }
        public decimal MobilizationFeePerKm { get; set; }
        public string? FrontPhotoUrl { get; set; }
        public string? SidePhotoUrl { get; set; }
        public string? SerialPlatePhotoUrl { get; set; }
        public string? FrontPhotoPublicId { get; set; }
        public string? SidePhotoPublicId { get; set; }
        public string? SerialPlatePublicId { get; set; }
        public string? Description { get; set; }
        public ICollection<AssetOperator> Operators { get; set; } = new List<AssetOperator>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
