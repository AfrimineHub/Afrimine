namespace Afrimine.Model.Entities
{
    public class WalletTransaction : BaseEntity
    {
        public Guid WalletId { get; set; }
        public SupplierWallet Wallet { get; set; } = null!;
        public string Type { get; set; } = string.Empty; // "credit" | "debit"
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string Currency { get; set; } = "NGN";
    }
}
