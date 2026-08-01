using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Booking : BaseEntity
    {
        public Guid AssetId { get; set; }
        public Asset Asset { get; set; } = null!;
        public string MinerId { get; set; } = string.Empty;
        public User Miner { get; set; } = null!;
        public Guid SupplierId { get; set; } = Guid.Empty;
        public SupplierProfile Supplier { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public double DistanceKm { get; set; }
        public decimal RentalFee { get; set; }
        public decimal MobilizationFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PlatformFee { get; set; }        // 15%
        public decimal SupplierPayout { get; set; }    // 85%
        public string? SiteAddress { get; set; }
        public double? SiteLatitude { get; set; }
        public double? SiteLongitude { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public string? DeclineReason { get; set; }
        public string? Currency { get; set; } = "NGN";

        // PayScrow integration
        public string? PayscrowTransactionId { get; set; }
        public string? PayscrowTransactionNumber { get; set; }
        public string? PayscrowPaymentLink { get; set; }
        public string? PayscrowTransactionReference { get; set; }
        public PayscrowTransactionStatus PayscrowStatus { get; set; } = PayscrowTransactionStatus.Pending;

        // Logistics
        public LogisticsStatus LogisticsStatus { get; set; } = LogisticsStatus.NotStarted;
        public string? LogisticsPartnerId { get; set; }
        public string? TrackingData { get; set; } // JSON from ETU
        public LogisticsType LogisticsType { get; set; } = LogisticsType.SupplierOwned;
        public string? ThirdPartyLogisticsRef { get; set; }

        // Insurance
        public bool GitInsuranceActive { get; set; } = false;  // auto on dispatch
        public string? InsurancePolicyNumber { get; set; }
        public string? InsuranceCertificateUrl { get; set; }
        public bool ParInsuranceActive { get; set; } = false;       // optional, buyer-initiated
        public string? ParInsurancePolicyNumber { get; set; }
        // Milestones
        public MilestoneStatus Milestone1Status { get; set; } = MilestoneStatus.Locked;
        public MilestoneStatus Milestone2Status { get; set; } = MilestoneStatus.Locked;
        public MilestoneStatus Milestone3Status { get; set; } = MilestoneStatus.Locked;
        public DateTime? Milestone1ReleasedAt { get; set; }
        public DateTime? Milestone2ReleasedAt { get; set; }
        public DateTime? Milestone3ReleasedAt { get; set; }
        public decimal Milestone1Amount { get; set; }  // 20% of supplier payout
        public decimal Milestone2Amount { get; set; }  // 40% of supplier payout
        public decimal Milestone3Amount { get; set; }  // 40% of supplier payout

        // Daily checks
        public ICollection<DailyCheck> DailyChecks { get; set; } = new List<DailyCheck>();
        public ICollection<BookingDispute> Disputes { get; set; } = new List<BookingDispute>();
    }
}
