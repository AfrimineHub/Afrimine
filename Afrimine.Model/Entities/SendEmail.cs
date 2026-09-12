using System.ComponentModel.DataAnnotations;

namespace Afrimine.Model.Entities
{
    public class SendEmail : BaseEntity
    {
        [Required]
        public string? Name { get; set; }

        public string? Email { get; set; }
    }
}
