namespace Afrimine.Services.DTOs
{
    public record AuthenticateResetPasswordOtpDto : OtpManipulationDto
    {
        public int Otp { get; init; }
    }
}
