namespace Afrimine.Model.Entities
{
    public class SupplierWallet : BaseEntity
    {
        public Guid SupplierId { get; set; } = Guid.Empty;
        public SupplierProfile Supplier { get; set; } = null!;
        public decimal AvailableBalance { get; set; } = 0;
        public decimal PendingBalance { get; set; } = 0;
        public string Currency { get; set; } = "NGN";
        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
