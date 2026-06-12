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
        public DbSet<ListingImage> ListingImages { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Revenue> Revenues { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Payout> Payouts { get; set; }

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

            builder.Entity<ListingImage>()
                .HasOne(x => x.Listing)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Subscription>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Revenue>()
                .HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Quote>()
                .HasOne(x => x.Vendor).WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Quote>()
                .HasOne(x => x.Buyer).WithMany()
                .HasForeignKey(x => x.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payout>()
                .HasOne(x => x.Vendor).WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}
