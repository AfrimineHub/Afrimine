using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IMarketService
    {
        Task<ApiResponse<IEnumerable<MarketTrendDto>>> GetMarketTrendsAsync();
        Task<ApiResponse<IEnumerable<InvestmentInsightDto>>> GetInvestmentInsightsAsync();
        Task<ApiResponse<PagedResultDto<RfqDto>>> GetOpenRfqsAsync(RfqQueryDto query);
        Task<ApiResponse<RfqDto>> GetRfqDetailAsync(Guid rfqId);
    }
}
