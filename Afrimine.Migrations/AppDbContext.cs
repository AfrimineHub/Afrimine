using Afrimine.Migrations.Configurations;
using Afrimine.Model.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Migrations
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { }

        public DbSet<OtpEntry> OtpEntries { get; set; }
        public DbSet<VendorProfile> VendorProfiles { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<SavedListing> SavedListings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("afrimine-api-dev");

            builder.ApplyConfiguration(new UserConfigurations());
            builder.ApplyConfiguration(new RoleConfiguration());

            builder.Entity<SavedListing>()
                .HasIndex(x => new { x.UserId, x.ListingId })
                .IsUnique();

            builder.Entity<Listing>()
                .HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(x => x.Buyer)
                .WithMany()
                .HasForeignKey(x => x.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(builder);
        }
    }
}
