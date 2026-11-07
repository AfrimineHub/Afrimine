using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using System.Text.Json;

namespace Afrimine.Services.BL.Implementation
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IRepositoryManager _repository;

        public SubscriptionService(IRepositoryManager repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<SubscriptionPlanDto>>> GetPlansAsync()
        {
            var plans = await _repository.SubscriptionPlan.GetAllPlansAsync();
            return ApiResponse<IEnumerable<SubscriptionPlanDto>>.Ok(plans.Select(p => new SubscriptionPlanDto
            {
                Id = p.PlanKey,
                Name = p.Name,
                Description = p.Description,
                PriceMonthly = p.PriceMonthly,
                Currency = p.Currency,
                ListingsLimit = p.ListingsLimit,
                IsPopular = p.IsPopular,
                Features = JsonSerializer.Deserialize<List<SubscriptionPlanFeatureDto>>(p.FeaturesJson)
                           ?? new List<SubscriptionPlanFeatureDto>()
            }));
        }

        public async Task<ApiResponse<CheckoutResponseDto>> CheckoutAsync(string userId, CheckoutRequestDto request)
        {
            var plan = await _repository.SubscriptionPlan.GetByKeyAsync(request.PlanId);
            if (plan is null) return ApiResponse<CheckoutResponseDto>.Fail("Plan not found.", 404);

            // TODO: Call Paystack API to generate real checkout URL
            var sessionId = $"cs_{Guid.NewGuid():N}";
            var checkoutUrl = $"https://paystack.com/pay/{sessionId}";

            await _repository.SubscriptionInvoice.Create(new SubscriptionInvoice
            {
                UserId = userId,
                PlanName = plan.Name,
                Amount = plan.PriceMonthly,
                Currency = plan.Currency,
                Status = InvoiceStatus.Pending,
                SessionId = sessionId
            });
            await _repository.SaveAsync();

            return ApiResponse<CheckoutResponseDto>.Ok(new CheckoutResponseDto
            {
                CheckoutUrl = checkoutUrl,
                SessionId = sessionId,
                Provider = "paystack"
            });
        }

        public async Task<ApiResponse<SubscriptionSummaryDto>> ChangePlanAsync(string userId, ChangePlanDto request)
        {
            var subscription = await _repository.Subscription.GetActiveByUserIdAsync(userId);
            if (subscription is null) return ApiResponse<SubscriptionSummaryDto>.Fail("No active subscription.", 404);

            subscription.PlanId = "free";
            subscription.PlanName = "FREE";
            subscription.ListingsLimit = 0;
            subscription.RenewsAt = DateTime.UtcNow;
            _repository.Subscription.Update(subscription);
            await _repository.SaveAsync();

            return ApiResponse<SubscriptionSummaryDto>.Ok(new SubscriptionSummaryDto
            {
                PlanId = "free",
                PlanName = "FREE",
                RenewsAt = subscription.RenewsAt
            });
        }

        public async Task<ApiResponse<SubscriptionSummaryDto>> CancelAsync(string userId, CancelSubscriptionDto request)
        {
            var subscription = await _repository.Subscription.GetActiveByUserIdAsync(userId);
            if (subscription is null) return ApiResponse<SubscriptionSummaryDto>.Fail("No active subscription.", 404);

            subscription.IsActive = false;
            _repository.Subscription.Update(subscription);
            await _repository.SaveAsync();

            return ApiResponse<SubscriptionSummaryDto>.Ok(new SubscriptionSummaryDto
            {
                PlanId = subscription.PlanId,
                PlanName = subscription.PlanName,
                RenewsAt = subscription.RenewsAt
            });
        }

        public async Task<ApiResponse<PagedResultDto<InvoiceDto>>> GetInvoicesAsync(
            string userId, int page, int pageSize)
        {
            var (items, total) = await _repository.SubscriptionInvoice.GetByUserAsync(userId, page, pageSize);
            return ApiResponse<PagedResultDto<InvoiceDto>>.Ok(new PagedResultDto<InvoiceDto>
            {
                Items = items.Select(i => new InvoiceDto
                {
                    Id = i.Id.ToString(),
                    Amount = i.Amount,
                    Currency = i.Currency,
                    Status = i.Status.ToString().ToLower(),
                    PlanName = i.PlanName,
                    PaidAt = i.PaidAt,
                    CreatedAt = i.CreatedAt,
                    InvoiceUrl = i.InvoiceUrl
                }),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            });
        }

        public async Task<ApiResponse<string>> ContactSalesAsync(string userId, ContactSalesDto request)
        {
            // TODO: Send notification to sales team
            return await Task.FromResult(ApiResponse<string>.Ok(
                "Your inquiry has been received. Our sales team will contact you shortly."));
        }

        public async Task<ApiResponse<SubscriptionSummaryDto>> VerifyCheckoutAsync(string userId, string sessionId)
        {
            var invoice = await _repository.SubscriptionInvoice.GetBySessionIdAsync(sessionId);
            if (invoice is null) return ApiResponse<SubscriptionSummaryDto>.Fail("Session not found.", 404);

            // TODO: Verify with Paystack API
            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidAt = DateTime.UtcNow;
            _repository.SubscriptionInvoice.Update(invoice);

            var subscription = await _repository.Subscription.GetActiveByUserIdAsync(userId);
            if (subscription is not null)
            {
                subscription.PlanId = invoice.PlanName.ToLower();
                subscription.PlanName = invoice.PlanName;
                subscription.RenewsAt = DateTime.UtcNow.AddMonths(1);
                _repository.Subscription.Update(subscription);
            }
            else
            {
                await _repository.Subscription.Create(new Subscription
                {
                    UserId = userId,
                    PlanId = invoice.PlanName.ToLower(),
                    PlanName = invoice.PlanName,
                    ListingsLimit = 10,
                    IsActive = true,
                    RenewsAt = DateTime.UtcNow.AddMonths(1)
                });
            }
            await _repository.SaveAsync();

            return ApiResponse<SubscriptionSummaryDto>.Ok(new SubscriptionSummaryDto
            {
                PlanId = invoice.PlanName.ToLower(),
                PlanName = invoice.PlanName,
                RenewsAt = DateTime.UtcNow.AddMonths(1)
            });
        }
    }
}
