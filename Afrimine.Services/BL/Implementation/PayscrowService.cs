using Afrimine.Model.ViewModels;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Shared.Configs;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Afrimine.Services.BL.Implementation
{
    public class PayscrowService : IPayscrowService
    {
        private readonly HttpClient _httpClient;
        private readonly AppConfig _config;

        public PayscrowService(HttpClient httpClient, IOptions<AppConfig> config)
        {
            _config = config.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_config.PayscrowBaseUrl);
            _httpClient.DefaultRequestHeaders.Add("BrokerApiKey", _config.PayscrowApiKey);
        }

        public async Task<PayscrowCreateTransactionResult> CreateTransactionAsync(PayscrowCreateTransactionRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/v3/marketplace/transactions/start", content);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(body);

                if (result.GetProperty("success").GetBoolean())
                {
                    var data = result.GetProperty("data");
                    return new PayscrowCreateTransactionResult
                    {
                        Success = true,
                        TransactionNumber = data.GetProperty("transactionNumber").GetString(),
                        PaymentLink = data.GetProperty("paymentLink").GetString(),
                        TotalPayable = data.GetProperty("totalPayable").GetDecimal(),
                        CurrencyCode = data.GetProperty("currencyCode").GetString()
                    };
                }

                return new PayscrowCreateTransactionResult
                {
                    Success = false,
                    Error = result.TryGetProperty("message", out var msg) ? msg.GetString() : "Unknown error"
                };
            }
            catch (Exception ex)
            {
                return new PayscrowCreateTransactionResult { Success = false, Error = ex.Message };
            }
        }

        public async Task<PayscrowTransactionStatusResult> GetTransactionStatusAsync(string transactionNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"/api/v3/marketplace/transactions/{transactionNumber}/status");
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(body);

                if (result.GetProperty("success").GetBoolean())
                {
                    var data = result.GetProperty("data");
                    return new PayscrowTransactionStatusResult
                    {
                        Success = true,
                        TransactionNumber = data.GetProperty("transactionNumber").GetString(),
                        Status = data.GetProperty("status").GetString(),
                        InEscrow = data.GetProperty("inEscrow").GetBoolean(),
                        InDispute = data.GetProperty("inDispute").GetBoolean()
                    };
                }

                return new PayscrowTransactionStatusResult
                {
                    Success = false,
                    Error = result.TryGetProperty("message", out var msg) ? msg.GetString() : "Unknown error"
                };
            }
            catch (Exception ex)
            {
                return new PayscrowTransactionStatusResult { Success = false, Error = ex.Message };
            }
        }

        public async Task<PayscrowChargesResult> CalculateChargesAsync(string currencyCode, decimal amount, decimal merchantChargePercentage = 0)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"/api/v3/marketplace/charges/calculate?currencyCode={currencyCode}&amount={amount}&merchantChargePercentage={merchantChargePercentage}");
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(body);

                if (result.GetProperty("success").GetBoolean())
                {
                    var data = result.GetProperty("data");
                    return new PayscrowChargesResult
                    {
                        Success = true,
                        Amount = data.GetProperty("amount").GetDecimal(),
                        TotalCharge = data.GetProperty("totalCharge").GetDecimal(),
                        MerchantCharge = data.GetProperty("merchantCharge").GetDecimal(),
                        CustomerCharge = data.GetProperty("customerCharge").GetDecimal(),
                        GrandTotalPayable = data.GetProperty("grandTotalPayable").GetDecimal(),
                        TotalSettlementAmount = data.GetProperty("totalSettlementAmount").GetDecimal(),
                        CurrencyCode = data.GetProperty("currencyCode").GetString(),
                        CurrencySymbol = data.GetProperty("currencySymbol").GetString()
                    };
                }

                return new PayscrowChargesResult
                {
                    Success = false,
                    Error = result.TryGetProperty("message", out var msg) ? msg.GetString() : "Unknown error"
                };
            }
            catch (Exception ex)
            {
                return new PayscrowChargesResult { Success = false, Error = ex.Message };
            }
        }

        public async Task<bool> RaiseDisputeAsync(string transactionNumber, string requestedBy, string complaint)
        {
            try
            {
                var payload = new { requestedBy, complaint };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(
                    $"/api/v3/marketplace/transactions/{transactionNumber}/broker/raise-dispute", content);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(body);
                return result.GetProperty("success").GetBoolean();
            }
            catch { return false; }
        }

        public async Task<bool> ApplyEscrowCodeAsync(string transactionId, string code)
        {
            try
            {
                var payload = new { transactionId, code };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(
                    "/api/v3/escrow/escrowtransactions/applycode", content);
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(body);
                return result.TryGetProperty("isSuccessful", out var s) && s.GetBoolean();
            }
            catch { return false; }
        }

        public async Task<IEnumerable<PayscrowBank>> GetSupportedBanksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    "/api/v3/payments/banks/broker/supported-banks");
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(body);
                if (result.GetProperty("success").GetBoolean())
                {
                    var banks = result.GetProperty("data").GetProperty("banks");
                    return banks.EnumerateArray().Select(b => new PayscrowBank
                    {
                        Name = b.GetProperty("name").GetString() ?? string.Empty,
                        Code = b.GetProperty("code").GetString() ?? string.Empty,
                        Country = b.GetProperty("country").GetString() ?? string.Empty
                    }).ToList();
                }
                return Enumerable.Empty<PayscrowBank>();
            }
            catch { return Enumerable.Empty<PayscrowBank>(); }
        }
    }
}
