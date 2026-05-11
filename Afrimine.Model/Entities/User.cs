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
    }
}
                                                      