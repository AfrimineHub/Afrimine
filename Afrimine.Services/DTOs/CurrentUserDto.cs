using Afrimine.Model.Enums;
using Afrimine.Shared.Extensions;

namespace Afrimine.Services.DTOs
{
    public class CurrentUserDto
    {
        public string Name { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
        public AccountStatus Status { get; set; }
        public string StatusText => Status.GetDescription();
    }
}
