using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Implementation
{
    public class MarketService : IMarketService
    {
        private readonly IRepositoryManager _repository;

        public MarketService(IRepositoryManager repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<MarketTrendDto>>> GetMarketTrendsAsync()
        {
            var trends = await _repository.MarketTrend.GetLatestAsync();
            return ApiResponse<IEnumerable<MarketTrendDto>>.Ok(trends.Select(t => new MarketTrendDto
            {
                Commodity = t.Commodity,
                Price = t.Price,
                Currency = t.Currency,
                Unit = t.Unit,
                ChangePercent = t.ChangePercent,
                AsOf = t.AsOf
            }));
        }

        public async Task<ApiResponse<IEnumerable<InvestmentInsightDto>>> GetInvestmentInsightsAsync()
        {
            var insights = await _repository.InvestmentInsight.GetLatestAsync();
            return ApiResponse<IEnumerable<InvestmentInsightDto>>.Ok(insights.Select(i => new InvestmentInsightDto
            {
                Id = i.Id,
                Title = i.Title,
                Summary = i.Summary,
                ImageUrl = i.ImageUrl,
                PublishedAt = i.PublishedAt
            }));
        }

        public async Task<ApiResponse<PagedResultDto<RfqDto>>> GetOpenRfqsAsync(RfqQueryDto query)
        {
            var (items, total) = await _repository.Rfq.GetOpenRfqsAsync(
                query.Page, query.PageSize, query.Status);

            return ApiResponse<PagedResultDto<RfqDto>>.Ok(new PagedResultDto<RfqDto>
            {
                Items = items.Select(r => new RfqDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    MineralType = r.MineralType,
                    Quantity = r.Quantity,
                    Unit = r.Unit,
                    TargetPrice = r.TargetPrice,
                    Location = r.Location,
                    Country = r.Country,
                    Status = r.Status.ToString(),
                    BuyerName = r.Buyer?.UserName ?? string.Empty,
                    ExpiresAt = r.ExpiresAt,
                    CreatedAt = r.CreatedAt
                }),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<ApiResponse<RfqDto>> GetRfqDetailAsync(Guid rfqId)
        {
            var rfq = await _repository.Rfq.GetByIdAsync(rfqId);
            if (rfq is null) return ApiResponse<RfqDto>.Fail("RFQ not found.", 404);

            return ApiResponse<RfqDto>.Ok(new RfqDto
            {
                Id = rfq.Id,
                Title = rfq.Title,
                Description = rfq.Description,
                MineralType = rfq.MineralType,
                Quantity = rfq.Quantity,
                Unit = rfq.Unit,
                TargetPrice = rfq.TargetPrice,
                Location = rfq.Location,
                Country = rfq.Country,
                Status = rfq.Status.ToString(),
                BuyerName = rfq.Buyer?.UserName ?? string.Empty,
                ExpiresAt = rfq.ExpiresAt,
                CreatedAt = rfq.CreatedAt
            });
        }
    }
}
