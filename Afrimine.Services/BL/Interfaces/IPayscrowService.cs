using Afrimine.Model.ViewModels;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IPayscrowService
    {
        Task<PayscrowCreateTransactionResult> CreateTransactionAsync(PayscrowCreateTransactionRequest request);
        Task<PayscrowTransactionStatusResult> GetTransactionStatusAsync(string transactionNumber);
        Task<PayscrowChargesResult> CalculateChargesAsync(string currencyCode, decimal amount, decimal merchantChargePercentage = 0);
        Task<bool> RaiseDisputeAsync(string transactionNumber, string requestedBy, string complaint);
        Task<bool> ApplyEscrowCodeAsync(string transactionId, string code);
        Task<IEnumerable<PayscrowBank>> GetSupportedBanksAsync();
    }
}