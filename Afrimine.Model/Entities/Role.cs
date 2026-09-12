using Microsoft.AspNetCore.Identity;

namespace Afrimine.Model.Entities
{
    public class Role : IdentityRole
    {
        public Role()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
