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
        public string Email { get; set; } = default!;
        [Required]
        public EToken Type { get; set; }
    }

    public record OtpForCreationDto : OtpManipulationDto
    {
        public string Otp { get; set; } = string.Empty;
    }
    public record ResendOtpRequestDto(string Email);
    }