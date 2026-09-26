namespace Afrimine.Services.DTOs
{
    // ── Escrow ────────────────────────────────────────────────────────────────
    public class AdminEscrowItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string? BuyerName { get; set; }
        public string? VendorName { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
        public string? PaymentReference { get; set; }
        public string? FundedAt { get; set; }
        public string? ReleasedAt { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class AdminEscrowQueryDto
    {
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
