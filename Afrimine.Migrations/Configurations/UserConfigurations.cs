using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Afrimine.Migrations.Configurations
{
    internal class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(30)
                .IsRequired();

            builder.HasIndex(u => u.PhoneNumber)
                .IsUnique();

            builder.Property(u => u.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(u => u.CreatedOn)
                .IsRequired();
        }
    }
}
