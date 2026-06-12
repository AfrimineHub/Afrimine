namespace Afrimine.Services.DTOs
{
    public class RevenueSummaryDto
    {
        public string Currency { get; set; } = "USD";
        public decimal TotalInflow { get; set; }
        public double TotalInflowChangePercent { get; set; }
        public decimal ThisMonthInflow { get; set; }
        public double ThisMonthChangePercent { get; set; }
    }
}
