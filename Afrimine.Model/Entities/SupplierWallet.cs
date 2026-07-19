namespace Afrimine.Model.Entities
{
    public class SupplierWallet : BaseEntity
    {
        public string SupplierId { get; set; } = string.Empty;
        public User Supplier { get; set; } = null!;
        public decimal AvailableBalance { get; set; } = 0;
        public decimal PendingBalance { get; set; } = 0;
        public string Currency { get; set; } = "NGN";
        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
