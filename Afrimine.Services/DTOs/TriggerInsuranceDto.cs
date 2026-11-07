using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{
    public class TriggerInsuranceDto
    {
        [Required]
        public string Type { get; set; } = "GIT"; // "GIT" | "PAR"
    }
}
