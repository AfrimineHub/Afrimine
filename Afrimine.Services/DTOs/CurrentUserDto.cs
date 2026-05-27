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

    public static class Roles
    {
        public const string Vendor = nameof(Role.Vendor);
        public const string Buyer = nameof(Role.Buyer);
        public const string Investor = nameof(Role.Investor);
        public const string Support = nameof(Role.Support);
        public const string SuperAdmin = nameof(Role.SuperAdmin);

        // Useful combinations
        public const string AdminOnly = $"{SuperAdmin}";
        public const string AdminAndSupport = $"{SuperAdmin},{Support}";
        public const string AllUsers = $"{Vendor},{Buyer},{Investor},{Support},{SuperAdmin}";
        public const string MarketParticipants = $"{Vendor},{Buyer},{Investor}";
    }
}
