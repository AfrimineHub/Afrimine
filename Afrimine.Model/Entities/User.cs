using Afrimine.Model.Enums;
using Microsoft.AspNetCore.Identity;

namespace Afrimine.Model.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = default!;
        public AccountStatus Status { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public RoleType Type { get; set; }
        public string? SuspendedReason { get; set; }
        public string? BannedReason { get; set; }
        public string? AvatarUrl { get; set; }
        public string? AvatarPublicId { get; set; }
    }
}
                                                      