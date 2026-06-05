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
        public RoleType Type { get; set; }
    }

    public static class Roles
    {
        public const string Vendor = nameof(RoleType.Vendor);
        public const string Buyer = nameof(RoleType.Buyer);
        public const string Investor = nameof(RoleType.Investor);
        public const string Supplier = nameof(RoleType.Supplier);
        public const string Support = nameof(RoleType.Support);
        public const string SuperAdmin = nameof(RoleType.SuperAdmin);

        // Useful combinations
        public const string AdminOnly = $"{SuperAdmin}";
        public const string AdminAndSupport = $"{SuperAdmin},{Support}";
        public const string AllUsers = $"{Vendor},{Buyer},{Investor},{Support},{SuperAdmin},{Supplier}";
        public const string MarketParticipants = $"{Vendor},{Buyer},{Investor}";
    }
}
