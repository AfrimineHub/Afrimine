using Afrimine.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{
    public class AdminStatItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public object Value { get; set; } = string.Empty;
        public string? Trend { get; set; }
        public bool? IsPositive { get; set; }
        public bool? IsNeutral { get; set; }
    }

    public class AdminAlertDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
    }

    public class AdminActivityDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class AdminOngoingTransactionDto
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string? BuyerName { get; set; }
        public string? VendorName { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
    }

    public class AdminDashboardDto
    {
        public List<AdminStatItemDto> Stats { get; set; } = new();
        public List<AdminAlertDto> PriorityAlerts { get; set; } = new();
        public List<AdminActivityDto> RecentActivity { get; set; } = new();
        public List<AdminOngoingTransactionDto> OngoingTransactions { get; set; } = new();
    }

    // ── Users ─────────────────────────────────────────────────────────────────
    public class AdminUserListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? KycStatus { get; set; }
        public string? AccountStatus { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class AdminUserStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int KycVerified { get; set; }
        public int Vendors { get; set; }
    }

    public class AdminUserQueryDto
    {
        public string? Q { get; set; }
        public string? Role { get; set; }
        public string? KycStatus { get; set; }
        public string? AccountStatus { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AdminUserActionDto
    {
        public string? Reason { get; set; }
    }

    // ── Listings ──────────────────────────────────────────────────────────────
    public class AdminListingListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Category { get; set; }
        public string? Location { get; set; }
        public string? SellerName { get; set; }
        public string? SellerEmail { get; set; }      
        public string? SupplierId { get; set; }       
        public string? CompanyName { get; set; }       
        public string? VendorType { get; set; }
        public string? Price { get; set; }
        public decimal? PriceAmount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class AdminListingCountsDto
    {
        public int All { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Flagged { get; set; }
    }

    public class AdminListingQueryDto
    {
        public string? Status { get; set; }
        public string? Q { get; set; }
        public Guid? SupplierId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AdminListingActionDto
    {
        public string? Reason { get; set; }
    }

    // ── Quotes ────────────────────────────────────────────────────────────────
    public class AdminQuoteListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string? ClientName { get; set; }
        public string? CompanyName { get; set; }
        public string? ProductName { get; set; }
        public string? Quantity { get; set; }
        public string? Status { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public bool? IsUnread { get; set; }
    }

    public class AdminQuoteQueryDto
    {
        public string? Q { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ── Orders ────────────────────────────────────────────────────────────────
    public class AdminOrderListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Source { get; set; } = "Order";
        public string? ListingTitle { get; set; }
        public string? Description { get; set; }
        public string? BuyerName { get; set; }
        public string? BuyerEmail { get; set; }
        public string? VendorName { get; set; }
        public string? VendorEmail { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class AdminOrderTimelineItemDto
    {
        public string Step { get; set; } = string.Empty;
        public string OccurredAt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class AdminOrderDocumentDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long? SizeBytes { get; set; }
        public string? UploadedBy { get; set; }
        public string UploadedAt { get; set; } = string.Empty;
        public string? DownloadUrl { get; set; }
    }

    public class AdminOrderDetailDto : AdminOrderListItemDto
    {
        public List<AdminOrderTimelineItemDto> Timeline { get; set; } = new();
        public List<AdminOrderDocumentDto> Documents { get; set; } = new();
    }

    public class AdminOrderSummaryDto
    {
        public int Total { get; set; }
        public int Completed { get; set; }
        public int InProgress { get; set; }
        public int Pending { get; set; }
        public int FailedOrCancelled { get; set; }
    }

    public class AdminOrderQueryDto
    {
        public string? Q { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ── Revenue ───────────────────────────────────────────────────────────────
    public class AdminRevenueSummaryDto
    {
        public decimal TotalPlatformRevenue { get; set; }
        public double? TotalPlatformRevenueChangePercent { get; set; }
        public decimal VendorPayouts { get; set; }
        public double? VendorPayoutsChangePercent { get; set; }
        public decimal PendingPayments { get; set; }
        public double? PendingPaymentsChangePercent { get; set; }
        public string? Currency { get; set; }
    }

    public class AdminTransactionItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string? VendorName { get; set; }
        public string? ProductName { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class AdminTransactionQueryDto
    {
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ── Withdrawals ───────────────────────────────────────────────────────────
    public class AdminWithdrawalItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string? VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? VendorEmail { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? BankName { get; set; }
        public string? Status { get; set; }
        public string RequestedAt { get; set; } = string.Empty;
    }

    public class AdminWithdrawalQueryDto
    {
        public string? Q { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AdminWithdrawalActionDto
    {
        public string? Reason { get; set; }
    }

    // ── KYC ───────────────────────────────────────────────────────────────────
    public class AdminKycQueueItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? DocumentType { get; set; }
        public string? Country { get; set; }
        public string SubmittedAt { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? CompanyName { get; set; }   // from SupplierProfile
    }

    public class AdminKycDetailDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentIdNumber { get; set; }
        public string? Country { get; set; }
        public string SubmittedAt { get; set; } = string.Empty;
        public string? ProfilePhotoUrl { get; set; }
        public string? DocumentFileName { get; set; }
        public long? DocumentFileSizeBytes { get; set; }
        //public string? DocumentDownloadUrl { get; set; }
        public List<KycDocumentDto> Documents { get; set; } = new();
        public string? Status { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class AdminKycQueryDto
    {
        public string? Q { get; set; }
        public string? Status { get; set; } = "pending";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AdminKycActionDto
    {
        public string? Reason { get; set; }
    }

    public class CreateAdminDto
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }

    public class KycDocumentDto
    {
        public string? FileName { get; set; }
        public string? DownloadUrl { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? DocumentType { get; set; }
    }


    public class AdminCreateUserDto
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required] public RoleType Role { get; set; }
        public VendorType? VendorType { get; set; }
    }

    public class AdminUpdateUserDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public RoleType? Role { get; set; }
        public AccountStatus? AccountStatus { get; set; }
    }
}