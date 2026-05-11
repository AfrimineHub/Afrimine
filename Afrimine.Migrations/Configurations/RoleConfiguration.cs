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
                    Name = Role.Vendor.ToString(),
                    NormalizedName = Role.Vendor.ToString().ToUpper(),
                    ConcurrencyStamp = DateTime.UtcNow.ToString(),
                },
                new IdentityRole
                {
                    Id = new Guid("FEAF882E-49D1-4047-B8D6-79BB1217B527").ToString(),
                    Name = Role.Buyer.ToString(),
                    NormalizedName = Role.Buyer.ToString().ToUpper(),
                    ConcurrencyStamp = DateTime.UtcNow.ToString(),
                },
                new IdentityRole
                {
                    Id = new Guid("FEAF882E-49D1-4047-B8D6-79BB1217B528").ToString(),
                    Name = Role.Support.ToString(),
                    NormalizedName = Role.Support.ToString().ToUpper(),
                    ConcurrencyStamp = DateTime.UtcNow.ToString(),
                },
                new IdentityRole
                {
                    Id = new Guid("FEAF882E-49D1-4047-B8D6-79BB1217B529").ToString(),
                    Name = Role.SuperAdmin.ToString(),
                    NormalizedName = Role.SuperAdmin.ToString().ToUpper(),
                    ConcurrencyStamp = DateTime.UtcNow.ToString(),
                }
            );
        }
    }
}
