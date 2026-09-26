namespace Afrimine.Model.Entities
{
    public class InvestmentInsight : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    }
}
