using Afrimine.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Afrimine.Model.Entities
{
    public class OtpEntry : BaseEntity
    {
        [Required]
        public string UserId { get; set; } = default!;
        [Required]
        public string OtpHash { get; set; } = default!;
        [Required]
        public DateTime ExpiresAt { get; set; }
        [Required]
        public EToken Type { get; set; }
        public string? TokenHash { get; set; }
        public int Attempts { get; set; }
    }
}
