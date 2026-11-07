using System.ComponentModel.DataAnnotations;

namespace Afrimine.Shared.RequestFeatures
{
    public record EmailParameters
    {
        [Required]
        public List<string> To { get; init; } = [];
        [Required]
        public string? Message { get; init; }
        [Required]
        public string? Subject { get; set; }
        public string Html { get; set; }
    }
}
