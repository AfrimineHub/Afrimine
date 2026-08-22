using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{
    public class SupplierDto
    {
        // ── Supplier Onboarding ───────────────────────────────────────────────────
        public class SupplierRegisterDto
        {
            [Required]
            public string FullName { get; set; } = string.Empty;
            [Required]
            public string CompanyName { get; set; } = string.Empty;
            [Required]
            public string BusinessPhone { get; set; } = string.Empty;
            [Required]
            [EmailAddress]
            public string BusinessEmail { get; set; } = string.Empty;
            [Required]
            public string Password { get; set; } = string.Empty;
        }

        public class SupplierProfileUpdateDto
        {
            public string? CompanyName { get; set; }
            public string? BusinessPhone { get; set; }
            public string? BusinessEmail { get; set; }
            [Required]
            public string BankName { get; set; } = string.Empty;
            [Required]
            public string? BankCode { get; set; } = string.Empty;
            [Required]
            public string BankAccountNumber { get; set; } = string.Empty;
            [Required]
            public string BankAccountName { get; set; } = string.Empty;
        }

        public class SupplierLocationDto
        {
            [Required]
            public string PrimaryBaseCity { get; set; } = string.Empty;
            [Required]
            public string YardAddress { get; set; } = string.Empty;
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
        }

        public class SupplierDocumentUploadDto
        {
            [Required]
            public IFormFile Document { get; set; } = null!;
        }

        public class SupplierStatusDto
        {
            public string Status { get; set; } = string.Empty;
            public string? RejectionReason { get; set; }
            public int OnboardingStep { get; set; }
            public bool IsSubmitted { get; set; }
        }

        public class SupplierProfileResponseDto
        {
            public string UserId { get; set; } = string.Empty;
            public string? CompanyName { get; set; }
            public string? BusinessPhone { get; set; }
            public string? BusinessEmail { get; set; }
            public string? PrimaryBaseCity { get; set; }
            public string? YardAddress { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public string? CacCertificateUrl { get; set; }
            public string Status { get; set; } = string.Empty;
            public int OnboardingStep { get; set; }
            public User User { get; set; } = null!;
            public string? CacCertificatePublicId { get; set; }
            public string? RejectionReason { get; set; }
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
            public BusinessType BusinessType { get; set; }
            public string StateOrRegion { get; set; } = string.Empty;
            public string? Website { get; set; }
            public VendorType VendorType { get; set; } = VendorType.EquipmentSupplier;
            public KycStatus KycStatus { get; set; } = KycStatus.NotStarted;
            public string? KycRejectionReason { get; set; }
            public bool IsComplete { get; set; } = false;
        }

        // ── Assets ────────────────────────────────────────────────────────────────
        public class CreateAssetDto
        {
            [Required]
            public MachineType MachineType { get; set; }
            [Required]
            public string Brand { get; set; } = string.Empty;
            [Required]
            public string Model { get; set; } = string.Empty;
            [Required]
            public int YearOfManufacture { get; set; }
            [Required]
            public int EngineHours { get; set; }
            public bool HasCertifiedOperator { get; set; } = false;
            public decimal DailyRentalRate { get; set; }
            public decimal MobilizationFeePerKm { get; set; }
            public string? Description { get; set; }
        }

        public class UpdateAssetDto
        {
            public string? Brand { get; set; }
            public string? Model { get; set; }
            public int? EngineHours { get; set; }
            public decimal? DailyRentalRate { get; set; }
            public decimal? MobilizationFeePerKm { get; set; }
            public string? Description { get; set; }
            public AssetStatus? Status { get; set; }
        }

        public class AssetPhotoUploadDto
        {
            public IFormFile? FrontPhoto { get; set; }
            public IFormFile? SidePhoto { get; set; }
            public IFormFile? SerialPlatePhoto { get; set; }
        }

        public class AssetResponseDto
        {
            public Guid Id { get; set; }
            public string MachineType { get; set; } = string.Empty;
            public string Brand { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public int YearOfManufacture { get; set; }
            public int EngineHours { get; set; }
            public bool HasCertifiedOperator { get; set; }
            public string Status { get; set; } = string.Empty;
            public decimal DailyRentalRate { get; set; }
            public decimal MobilizationFeePerKm { get; set; }
            public string? FrontPhotoUrl { get; set; }
            public string? SidePhotoUrl { get; set; }
            public string? SerialPlatePhotoUrl { get; set; }
            public string? Description { get; set; }
            public string? SupplierCity { get; set; }
            public string? SupplierYardAddress { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class AssetPricingDto
        {
            public int TotalDays { get; set; }
            public double DistanceKm { get; set; }
            public decimal DailyRentalRate { get; set; }
            public decimal RentalFee { get; set; }
            public decimal MobilizationFee { get; set; }
            public decimal PlatformFee { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal SupplierPayout { get; set; }
            public string Currency { get; set; } = "NGN";
            // PayScrow charges preview
            public decimal PayscrowCharge { get; set; }
            public decimal TotalPayable { get; set; }
        }

        // ── Operators ─────────────────────────────────────────────────────────────
        public class CreateOperatorDto
        {
            [Required]
            public string FullName { get; set; } = string.Empty;
            [Required]
            public string PhoneNumber { get; set; } = string.Empty;
            [Required] 
            public string LicenseNumber { get; set; } = string.Empty;
            public string LicenseCategory { get; set; } = "E";
            [Required]
            public int YearsOfExperience { get; set; }
            public IFormFile? LicenseDocument { get; set; }
        }

        public class OperatorResponseDto
        {
            public Guid Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string LicenseNumber { get; set; } = string.Empty;
            public string LicenseCategory { get; set; } = string.Empty;
            public int YearsOfExperience { get; set; }
            public string VettingStatus { get; set; } = string.Empty;
            public bool PassedVetting { get; set; }
            public string? LicenseDocumentUrl { get; set; }
            public List<GuarantorDto> Guarantors { get; set; } = new();
        }

        public class CreateGuarantorDto
        {
            [Required]
            public string FullName { get; set; } = string.Empty;
            [Required]
            public string PhoneNumber { get; set; } = string.Empty;
            [Required]
            public string Occupation { get; set; } = string.Empty;
            [Required]
            public string IdType { get; set; } = string.Empty;
            [Required]
            public string IdNumber { get; set; } = string.Empty;
        }

        public class GuarantorDto
        {
            public Guid Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Occupation { get; set; } = string.Empty;
            public string IdType { get; set; } = string.Empty;
        }

        public class VettingSubmitDto
        {
            [Required]
            public string TerrainKnowledgeAnswer { get; set; } = string.Empty;
            [Required]
            public string DailyMaintenanceAnswer { get; set; } = string.Empty;
            public string? AdditionalNotes { get; set; }
        }

        public class VettingStatusDto
        {
            public string Status { get; set; } = string.Empty;
            public bool PassedVetting { get; set; }
            public string? Feedback { get; set; }
        }

        // ── Bookings ──────────────────────────────────────────────────────────────
        public class CreateBookingDto
        {
            [Required]
            public Guid AssetId { get; set; }
            [Required]
            public DateTime StartDate { get; set; }
            [Required]
            public DateTime EndDate { get; set; }
            [Required]
            public string SiteAddress { get; set; } = string.Empty;
            public double? SiteLatitude { get; set; }
            public double? SiteLongitude { get; set; }
            public double DistanceKm { get; set; }
            public string Currency { get; set; } = "NGN";
            // Miner's phone for PayScrow
            [Required] public string MinerPhone { get; set; } = string.Empty;
            public LogisticsType LogisticsType { get; set; } = LogisticsType.SupplierOwned;
        }

        public class BookingResponseDto
        {
            public Guid Id { get; set; }
            public Guid AssetId { get; set; }
            public string AssetName { get; set; } = string.Empty;
            public string MinerName { get; set; } = string.Empty;
            public string SupplierName { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int TotalDays { get; set; }
            public decimal RentalFee { get; set; }
            public decimal MobilizationFee { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal PlatformFee { get; set; }
            public decimal SupplierPayout { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? PaymentLink { get; set; }
            public string? PayscrowTransactionNumber { get; set; }
            public string? SiteAddress { get; set; }
            public string Currency { get; set; } = "NGN";
            public DateTime CreatedAt { get; set; }
        }

        public class BookingDetailDto : BookingResponseDto
        {
            public string LogisticsStatus { get; set; } = string.Empty;
            public bool GitInsuranceActive { get; set; }
            public string? InsurancePolicyNumber { get; set; }
            public MilestoneDto Milestone1 { get; set; } = new();
            public MilestoneDto Milestone2 { get; set; } = new();
            public MilestoneDto Milestone3 { get; set; } = new();
            public PaymentBreakdownDto PaymentBreakdown { get; set; } = new();
        }

        public class MilestoneDto
        {
            public string Name { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime? ReleasedAt { get; set; }
            public string Description { get; set; } = string.Empty;
        }

        public class PaymentBreakdownDto
        {
            public decimal TotalEscrow { get; set; }
            public decimal PlatformFee { get; set; }       // 15%
            public decimal SupplierShare { get; set; }     // 85%
            public decimal Milestone1Amount { get; set; }  // 20%
            public decimal Milestone2Amount { get; set; }  // 40%
            public decimal Milestone3Amount { get; set; }  // 40%
            public string Currency { get; set; } = "NGN";
        }

        public class DeclineBookingDto
        {
            [Required]
            public string Reason { get; set; } = string.Empty;
        }

        // ── Logistics ─────────────────────────────────────────────────────────────
        public class LogisticsStatusDto
        {
            public string Status { get; set; } = string.Empty;
            public string? TrackingData { get; set; }
            public bool GitInsuranceActive { get; set; }
            public string? InsuranceCertificateUrl { get; set; }
        }

        public class TrackingDto
        {
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public string? LastUpdated { get; set; }
            public string? Status { get; set; }
        }

        // ── Daily Check ───────────────────────────────────────────────────────────
        public class DailyCheckDto
        {
            [Required]
            public bool EngineOilChecked { get; set; }
            [Required]
            public bool HydraulicFluidChecked { get; set; }
            [Required]
            public bool CoolingSystemChecked { get; set; }
            [Required]
            public bool UndercarriageChecked { get; set; }
            [Required]
            public bool GreaseChecked { get; set; }
            public string? Notes { get; set; }
            [Required]
            public string CheckedByOperatorId { get; set; } = string.Empty;
        }

        // ── Dispute ───────────────────────────────────────────────────────────────
        public class BookingDisputeDto
        {
            [Required]
            public string Description { get; set; } = string.Empty;
            [Required]
            public string RaisedByRole { get; set; } = string.Empty; // "miner" | "supplier"
        }

        public class BookingDisputeResponseDto
        {
            public Guid Id { get; set; }
            public string RaisedByRole { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string? Resolution { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ── Wallet ────────────────────────────────────────────────────────────────
        public class WalletBalanceDto
        {
            public decimal AvailableBalance { get; set; }
            public decimal PendingBalance { get; set; }
            public string Currency { get; set; } = "NGN";
        }

        public class WithdrawalRequestDto
        {
            [Required]
            public decimal Amount { get; set; }
            public string Currency { get; set; } = "NGN";
        }

        public class WalletTransactionDto
        {
            public Guid Id { get; set; }
            public string Type { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Description { get; set; } = string.Empty;
            public string? Reference { get; set; }
            public string Currency { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
        }

        // ── Supplier Dashboard ────────────────────────────────────────────────────
        public class SupplierDashboardStatsDto
        {
            public int TotalMachines { get; set; }
            public int ActiveLeases { get; set; }
            public decimal CurrentMonthEarnings { get; set; }
            public decimal PendingEscrow { get; set; }
            public string Currency { get; set; } = "NGN";
        }

        // ── Webhook ───────────────────────────────────────────────────────────────
        public class PayscrowWebhookPayload
        {
            public string TransactionId { get; set; } = string.Empty;
            public string TransactionNumber { get; set; } = string.Empty;
            public string ExternalReference { get; set; } = string.Empty;
            public string PaymentStatus { get; set; } = string.Empty;
            public string EscrowStatus { get; set; } = string.Empty;
            public string AmountPaid { get; set; } = string.Empty;
            public string Currency { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string PaymentDate { get; set; } = string.Empty;
            public string Timestamp { get; set; } = string.Empty;
        }

        public class EscrowCodeApplyDto
        {
            [Required]
            public string TransactionId { get; set; } = string.Empty;
            [Required]
            public string Code { get; set; } = string.Empty;
        }

        public class AdminMilestoneItemDto
        {
            public string BookingId { get; set; } = string.Empty;
            public int MilestoneNumber { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? ReleasedAt { get; set; }
            public string? MinerName { get; set; }
            public string? SupplierName { get; set; }
            public string? BookingStatus { get; set; }
        }

        public class AdminMilestoneQueryDto
        {
            public string? Status { get; set; }   // Locked / Pending / Released
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }
    }
}
