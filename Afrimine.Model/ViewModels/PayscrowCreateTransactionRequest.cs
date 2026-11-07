namespace Afrimine.Model.ViewModels
{
    public class PayscrowCreateTransactionRequest
    {
        public string TransactionReference { get; set; } = string.Empty;
        public string MerchantEmailAddress { get; set; } = string.Empty;
        public string? MerchantPhoneNo { get; set; }
        public string MerchantName { get; set; } = string.Empty;
        public string CustomerEmailAddress { get; set; } = string.Empty;
        public string CustomerPhoneNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = "NGN";
        public decimal MerchantChargePercentage { get; set; } = 0;
        public string? ReturnUrl { get; set; }
        public string? WebhookNotificationUrl { get; set; }
        public List<PayscrowItem> Items { get; set; } = new();
        public List<PayscrowSettlementAccount>? SettlementAccounts { get; set; }
    }

    public class PayscrowItem
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
    }

    public class PayscrowSettlementAccount
    {
        public string BankCode { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class PayscrowCreateTransactionResult
    {
        public bool Success { get; set; }
        public string? TransactionNumber { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentLink { get; set; }
        public decimal TotalPayable { get; set; }
        public string? CurrencyCode { get; set; }
        public string? Error { get; set; }
    }

    public class PayscrowTransactionStatusResult
    {
        public bool Success { get; set; }
        public string? TransactionNumber { get; set; }
        public string? Status { get; set; }
        public bool InEscrow { get; set; }
        public bool InDispute { get; set; }
        public string? Error { get; set; }
    }

    public class PayscrowChargesResult
    {
        public bool Success { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalCharge { get; set; }
        public decimal MerchantCharge { get; set; }
        public decimal CustomerCharge { get; set; }
        public decimal GrandTotalPayable { get; set; }
        public decimal TotalSettlementAmount { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }
        public string? Error { get; set; }
    }

    public class PayscrowBank
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
