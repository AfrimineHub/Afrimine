using Afrimine.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{

    public record OtpDto : OtpManipulationDto
    {

    }
    public abstract record OtpManipulationDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public EToken Type { get; set; }
    }

    public record OtpForCreationDto : OtpManipulationDto
    {
        public string Otp { get; set; }
        public string Email { get; set; } = default!;
    }
}