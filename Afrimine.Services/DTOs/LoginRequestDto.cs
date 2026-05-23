namespace Afrimine.Services.DTOs
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public record LoginResponseDto(string AccessToken, string RefreshToken);
    public record RefreshTokenRequestDto(string AccessToken, string RefreshToken);
}
