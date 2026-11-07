namespace Afrimine.Model.Entities
{
    public class MarketTrend : BaseEntity
    {
        public string Commodity { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public string Unit { get; set; } = string.Empty;
        public double ChangePercent { get; set; }
        public DateTime AsOf { get; set; } = DateTime.UtcNow;
    }
}
