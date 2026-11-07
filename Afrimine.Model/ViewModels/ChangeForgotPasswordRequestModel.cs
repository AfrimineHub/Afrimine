using System.ComponentModel.DataAnnotations;

namespace Afrimine.Model.ViewModels
{
    public class ChangeForgotPasswordRequestModel
    {
        [Required]
        public string Otp { get; set; } = default!;
        [Required]
        public string Email { get; init; } = default!;
        [Required]
        public string NewPassword { get; init; } = default!;
        [Required, Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; } = default!;
    }
}
