using Afrimine.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public record LoginResponseDto(string AccessToken);
    public record RefreshTokenRequestDto(string AccessToken);

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required, MinLength(8)] 
        public string NewPassword { get; set; } = string.Empty;
        [Required, Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class GoogleLoginRequestDto
    {
        /// <summary>The ID token (JWT) returned by Google Sign-In on the client.</summary>
        [Required]
        public string IdToken { get; set; } = default!;

        /// <summary>
        /// Role to assign if this Google sign-in creates a brand new account.
        /// Ignored if the account already exists. Defaults to Buyer.
        /// </summary>
        public RoleType Type { get; set; } = RoleType.Buyer;
    }
}
