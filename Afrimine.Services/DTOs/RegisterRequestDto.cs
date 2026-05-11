using Afrimine.Model.Enums;

namespace Afrimine.Services.DTOs
{
    public class RegisterRequestDto
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public Role Role { get; set; }

        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }
}
