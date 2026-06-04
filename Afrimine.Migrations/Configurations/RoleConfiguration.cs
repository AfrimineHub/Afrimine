using Afrimine.Model.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Afrimine.Migrations.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = new Guid("FEAF882E-49D1-4047-B8D6-79BB1217B526").ToString(),
                    Name = RoleType.Vendor.ToString(),
                    NormalizedName = RoleType.Vendor.ToString().ToUpper(),
                    ConcurrencyStamp = DateTime.UtcNow.ToString(),
                },
                new IdentityRole
                {
                    Id = new Guid("FEAF882E-49D1-4047-B8D6-79BB1217B527").ToString(),
                    Name = RoleType.Buyer.ToString(),
                    NormalizedName = RoleType.Buyer.ToString().ToUpper(),
                    ConcurrencyStamp = DateTime.UtcNow.ToString(),
                },
                new IdentityRole
                {
                    Id = new Guid("FEAF882E-49D1-4047-B8D6-79BB1217B528").ToString(),
                    Name = RoleType.Support.ToString(),
                    NormalizedName = RoleType.Support.ToString().ToUpper(),
                    ConcurrencyStamp = DateTime.UtcNow.ToString(),
                },
                new IdentityRole
                {
                    Id = new Guid("FEAF882E-49D1-4047-B8D6-79BB1217B529").ToString(),
                    Name = RoleType.SuperAdmin.ToString(),
                    NormalizedName = RoleType.SuperAdmin.ToString().ToUpper(),
                    ConcurrencyStamp = DateTime.UtcNow.ToString(),
                }
            );
        }
    }
}
