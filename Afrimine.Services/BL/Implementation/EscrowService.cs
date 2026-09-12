using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Implementation
{
    public class EscrowService : IEscrowService
    {
        private readonly IRepositoryManager _repository;

        public EscrowService(IRepositoryManager repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<EscrowStatusDto>> GetEscrowStatusAsync(string userId, Guid orderId)
        {
            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is null) return ApiResponse<EscrowStatusDto>.Fail("Order not found.", 404);
            if (order.BuyerId != userId) return ApiResponse<EscrowStatusDto>.Fail("Access denied.", 403);

            var escrow = await _repository.Escrow.GetByOrderIdAsync(orderId);
            if (escrow is null) return ApiResponse<EscrowStatusDto>.Fail("No escrow found for this order.", 404);

            return ApiResponse<EscrowStatusDto>.Ok(MapToEscrowDto(escrow));
        }

        public async Task<ApiResponse<EscrowCheckoutResponseDto>> CheckoutEscrowAsync(string buyerId, Guid orderId)
        {
            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is null) return ApiResponse<EscrowCheckoutResponseDto>.Fail("Order not found.", 404);
            if (order.BuyerId != buyerId) return ApiResponse<EscrowCheckoutResponseDto>.Fail("Access denied.", 403);

            var existing = await _repository.Escrow.GetByOrderIdAsync(orderId);
            if (existing is not null && existing.Status == EscrowStatus.Funded)
                return ApiResponse<EscrowCheckoutResponseDto>.Fail("Escrow already funded.", 409);

            // TODO: Call Paystack to generate real checkout URL
            var sessionId = $"esc_{Guid.NewGuid():N}";
            var checkoutUrl = $"https://paystack.com/pay/{sessionId}";

            if (existing is null)
            {
                await _repository.Escrow.Create(new Escrow
                {
                    OrderId = orderId,
                    BuyerId = buyerId,
                    VendorId = order.VendorId,
                    Amount = order.Amount,
                    Currency = order.Currency,
                    Status = EscrowStatus.Pending,
                    CheckoutUrl = checkoutUrl,
                    SessionId = sessionId
                });
                await _repository.SaveAsync();
            }

            return ApiResponse<EscrowCheckoutResponseDto>.Ok(new EscrowCheckoutResponseDto
            {
                CheckoutUrl = checkoutUrl,
                SessionId = sessionId,
                Provider = "paystack"
            });
        }

        public async Task<ApiResponse<EscrowStatusDto>> VerifyEscrowPaymentAsync(
            string buyerId, Guid orderId, string sessionId)
        {
            var escrow = await _repository.Escrow.GetByOrderIdAsync(orderId);
            if (escrow is null) return ApiResponse<EscrowStatusDto>.Fail("Escrow not found.", 404);

            // TODO: Verify with Paystack
            escrow.Status = EscrowStatus.Funded;
            escrow.FundedAt = DateTime.UtcNow;
            escrow.PaymentReference = sessionId;
            _repository.Escrow.Update(escrow);

            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is not null)
            {
                order.Status = OrderStatus.Paid;
                order.PaidAt = DateTime.UtcNow;
                _repository.Order.Update(order);
            }

            await _repository.SaveAsync();
            return ApiResponse<EscrowStatusDto>.Ok(MapToEscrowDto(escrow));
        }

        public async Task<ApiResponse<EscrowStatusDto>> GetVendorEscrowStatusAsync(string vendorId, Guid orderId)
        {
            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is null) return ApiResponse<EscrowStatusDto>.Fail("Order not found.", 404);
            if (order.VendorId != vendorId) return ApiResponse<EscrowStatusDto>.Fail("Access denied.", 403);

            var escrow = await _repository.Escrow.GetByOrderIdAsync(orderId);
            if (escrow is null) return ApiResponse<EscrowStatusDto>.Fail("No escrow found.", 404);

            return ApiResponse<EscrowStatusDto>.Ok(MapToEscrowDto(escrow));
        }

        public async Task<ApiResponse<string>> MarkDeliveredAsync(string vendorId, Guid orderId)
        {
            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is null) return ApiResponse<string>.Fail("Order not found.", 404);
            if (order.VendorId != vendorId) return ApiResponse<string>.Fail("Access denied.", 403);
            if (order.Status != OrderStatus.Paid)
                return ApiResponse<string>.Fail("Order must be Paid before marking as delivered.", 409);

            order.Status = OrderStatus.Delivered;
            order.DeliveredAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            _repository.Order.Update(order);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Order marked as delivered.");
        }

        public async Task<ApiResponse<string>> SubmitRfqQuoteAsync(
            string vendorId, Guid rfqId, VendorQuoteSubmitDto request)
        {
            var rfq = await _repository.Rfq.GetByIdAsync(rfqId);
            if (rfq is null) return ApiResponse<string>.Fail("RFQ not found.", 404);
            if (rfq.Status != RfqStatus.Open)
                return ApiResponse<string>.Fail("RFQ is no longer open.", 409);

            await _repository.RfqQuote.Create(new RfqQuote
            {
                RfqId = rfqId,
                VendorId = vendorId,
                Amount = request.Amount,
                Currency = request.Currency,
                Note = request.Note,
                Status = QuoteStatus.Pending
            });
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Quote submitted successfully.");
        }

        public async Task<ApiResponse<string>> RequestPayoutAsync(string vendorId, PayoutRequestDto request)
        {
            await _repository.Payout.Create(new Payout
            {
                VendorId = vendorId,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = PayoutStatus.Pending
            });
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Payout request submitted.");
        }

        public async Task<ApiResponse<IEnumerable<RfqQuoteDto>>> GetRfqQuotesAsync(string buyerId, Guid rfqId)
        {
            var rfq = await _repository.Rfq.GetByIdAsync(rfqId);
            if (rfq is null) return ApiResponse<IEnumerable<RfqQuoteDto>>.Fail("RFQ not found.", 404);
            if (rfq.BuyerId != buyerId) return ApiResponse<IEnumerable<RfqQuoteDto>>.Fail("Access denied.", 403);

            var quotes = await _repository.RfqQuote.GetByRfqIdAsync(rfqId);
            return ApiResponse<IEnumerable<RfqQuoteDto>>.Ok(quotes.Select(q => new RfqQuoteDto
            {
                Id = q.Id,
                RfqId = q.RfqId,
                VendorName = q.Vendor?.UserName ?? string.Empty,
                Amount = q.Amount,
                Currency = q.Currency,
                Note = q.Note,
                Status = q.Status.ToString(),
                CreatedAt = q.CreatedAt
            }));
        }

        public async Task<ApiResponse<string>> AcceptRfqQuoteAsync(string buyerId, Guid rfqId, Guid quoteId)
        {
            var rfq = await _repository.Rfq.GetByIdAsync(rfqId);
            if (rfq is null) return ApiResponse<string>.Fail("RFQ not found.", 404);
            if (rfq.BuyerId != buyerId) return ApiResponse<string>.Fail("Access denied.", 403);

            var quote = await _repository.RfqQuote.GetByIdAsync(quoteId);
            if (quote is null) return ApiResponse<string>.Fail("Quote not found.", 404);

            quote.Status = QuoteStatus.Accepted;
            _repository.RfqQuote.Update(quote);

            rfq.Status = RfqStatus.Awarded;
            _repository.Rfq.Update(rfq);

            // Create order from accepted quote
            await _repository.Order.Create(new Order
            {
                BuyerId = buyerId,
                VendorId = quote.VendorId,
                ListingId = Guid.Empty, // RFQ orders may not have a listing
                Amount = quote.Amount,
                Currency = quote.Currency,
                Status = OrderStatus.Pending
            });

            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Quote accepted. Order created.");
        }

        public async Task<ApiResponse<PagedResultDto<DisputeDto>>> GetDisputesAsync(
            int page, int pageSize, string? status)
        {
            DisputeStatus? disputeStatus = null;
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<DisputeStatus>(status, true, out var parsed))
                disputeStatus = parsed;

            var (items, total) = await _repository.Dispute.GetAllAsync(page, pageSize, disputeStatus);
            return ApiResponse<PagedResultDto<DisputeDto>>.Ok(new PagedResultDto<DisputeDto>
            {
                Items = items.Select(d => new DisputeDto
                {
                    Id = d.Id,
                    OrderId = d.OrderId,
                    ListingTitle = d.Order?.Listing?.Title ?? string.Empty,
                    RaisedByName = d.RaisedBy?.UserName ?? string.Empty,
                    Reason = d.Reason,
                    Status = d.Status.ToString(),
                    AdminNote = d.AdminNote,
                    ResolvedAt = d.ResolvedAt,
                    CreatedAt = d.CreatedAt
                }),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            });
        }

        public async Task<ApiResponse<DisputeDto>> GetDisputeByIdAsync(Guid disputeId)
        {
            var d = await _repository.Dispute.GetByIdAsync(disputeId);
            if (d is null) return ApiResponse<DisputeDto>.Fail("Dispute not found.", 404);

            return ApiResponse<DisputeDto>.Ok(new DisputeDto
            {
                Id = d.Id,
                OrderId = d.OrderId,
                ListingTitle = d.Order?.Listing?.Title ?? string.Empty,
                RaisedByName = d.RaisedBy?.UserName ?? string.Empty,
                Reason = d.Reason,
                Status = d.Status.ToString(),
                AdminNote = d.AdminNote,
                ResolvedAt = d.ResolvedAt,
                CreatedAt = d.CreatedAt
            });
        }

        public async Task<ApiResponse<string>> ResolveDisputeAsync(
            string adminId, Guid disputeId, ResolveDisputeDto request)
        {
            var dispute = await _repository.Dispute.GetByIdAsync(disputeId);
            if (dispute is null) return ApiResponse<string>.Fail("Dispute not found.", 404);

            dispute.Status = request.Resolution.ToLower() == "buyer"
                ? DisputeStatus.ResolvedBuyer
                : DisputeStatus.ResolvedVendor;
            dispute.AdminNote = request.AdminNote;
            dispute.ResolvedById = adminId;
            dispute.ResolvedAt = DateTime.UtcNow;
            _repository.Dispute.Update(dispute);

            // Release or refund escrow based on resolution
            var escrow = await _repository.Escrow.GetByOrderIdAsync(dispute.OrderId);
            if (escrow is not null)
            {
                escrow.Status = request.Resolution.ToLower() == "vendor"
                    ? EscrowStatus.Released
                    : EscrowStatus.Refunded;
                escrow.ReleasedAt = DateTime.UtcNow;
                _repository.Escrow.Update(escrow);
            }

            await _repository.SaveAsync();
            return ApiResponse<string>.Ok($"Dispute resolved in favour of {request.Resolution}.");
        }

        public async Task<ApiResponse<string>> FreezeOrderAsync(string adminId, Guid orderId)
        {
            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is null) return ApiResponse<string>.Fail("Order not found.", 404);

            order.Status = OrderStatus.Frozen;
            _repository.Order.Update(order);

            var escrow = await _repository.Escrow.GetByOrderIdAsync(orderId);
            if (escrow is not null)
            {
                escrow.Status = EscrowStatus.Frozen;
                escrow.FrozenAt = DateTime.UtcNow;
                _repository.Escrow.Update(escrow);
            }

            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Order and escrow frozen.");
        }

        public async Task<ApiResponse<string>> ReleaseFundsAsync(string adminId, Guid orderId)
        {
            var escrow = await _repository.Escrow.GetByOrderIdAsync(orderId);
            if (escrow is null) return ApiResponse<string>.Fail("Escrow not found.", 404);

            escrow.Status = EscrowStatus.Released;
            escrow.ReleasedAt = DateTime.UtcNow;
            _repository.Escrow.Update(escrow);

            var order = await _repository.Order.GetByIdAsync(orderId);
            if (order is not null)
            {
                order.Status = OrderStatus.Completed;
                _repository.Order.Update(order);
            }

            await _repository.SaveAsync();
            return ApiResponse<string>.Ok("Funds released to vendor.");
        }

        private static EscrowStatusDto MapToEscrowDto(Escrow e) => new()
        {
            OrderId = e.OrderId,
            Status = e.Status.ToString(),
            Amount = e.Amount,
            Currency = e.Currency,
            PaymentReference = e.PaymentReference,
            FundedAt = e.FundedAt,
            ReleasedAt = e.ReleasedAt,
            FrozenAt = e.FrozenAt
        };
    }
}
