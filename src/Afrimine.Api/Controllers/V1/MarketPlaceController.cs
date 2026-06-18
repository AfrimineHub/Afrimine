using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Shared.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Afrimine.Api.Controllers.V1
{
    [Route("api/v{version:apiversion}")]
    [ApiVersion("1.0")]
    [ApiController]
    public class MarketPlaceController : ControllerBase
    {
        private readonly IServiceManager _service;

        public MarketPlaceController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>Search and browse marketplace listings (public)</summary>
        [AllowAnonymous]
        [HttpGet("listings")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<MarketplaceListingDto>>), 200)]
        public async Task<IActionResult> SearchListings([FromQuery] MarketplaceQueryDto query)
        {
            var response = await _service.Buyer.SearchListingsAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get single listing detail (public)</summary>
        [AllowAnonymous]
        [HttpGet("listings/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<MarketplaceListingDetailDto>), 200)]
        public async Task<IActionResult> GetListing(Guid id)
        {
            var viewerId = HttpContext.User.GetLoggedInUserId();
            var response = await _service.Buyer.GetListingDetailAsync(id, viewerId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get available listing categories (public)</summary>
        [AllowAnonymous]
        [HttpGet("listings/categories")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), 200)]
        public async Task<IActionResult> GetCategories()
        {
            var response = await _service.Buyer.GetCategoriesAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Inquire on a listing / contact seller</summary>
        [Authorize(Roles = Roles.Buyer)]
        [HttpPost("listings/{id:guid}/inquire")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> InquireListing(Guid id, [FromBody] CreateInquiryDto request)
        {
            var buyerId = HttpContext.User.GetLoggedInUserId();
            if (string.IsNullOrWhiteSpace(buyerId)) return Unauthorized();
            var response = await _service.Buyer.InquireListingAsync(buyerId, id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get market trends (public)</summary>
        [AllowAnonymous]
        [HttpGet("dashboard/market-trends")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MarketTrendDto>>), 200)]
        public async Task<IActionResult> GetMarketTrends()
        {
            var response = await _service.Market.GetMarketTrendsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get investment insights (public)</summary>
        [AllowAnonymous]
        [HttpGet("dashboard/investment-insights")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InvestmentInsightDto>>), 200)]
        public async Task<IActionResult> GetInvestmentInsights()
        {
            var response = await _service.Market.GetInvestmentInsightsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Browse open RFQs — vendors view buyer requests</summary>
        [AllowAnonymous]
        [HttpGet("rfqs")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<RfqDto>>), 200)]
        public async Task<IActionResult> GetOpenRfqs([FromQuery] RfqQueryDto query)
        {
            var response = await _service.Market.GetOpenRfqsAsync(query);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>Get single RFQ detail (public)</summary>
        [AllowAnonymous]
        [HttpGet("rfqs/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<RfqDto>), 200)]
        public async Task<IActionResult> GetRfqDetail(Guid id)
        {
            var response = await _service.Market.GetRfqDetailAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
