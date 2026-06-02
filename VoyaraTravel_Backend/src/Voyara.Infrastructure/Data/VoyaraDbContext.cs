using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Voyara.Core.Entities;

namespace Voyara.Infrastructure.Data
{
    public class VoyaraDbContext(DbContextOptions<VoyaraDbContext> options)
      : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingTraveler> Travelers { get; set; }
        public DbSet<BookingAddon> Addons { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all IEntityTypeConfiguration classes automatically
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(VoyaraDbContext).Assembly);

            base.OnModelCreating(modelBuilder);

            // 1. Define the Comparer for List<string>
            var stringListComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!), // Compares the actual items in the lists
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Generates a unique hash
                c => c.ToList()); // Creates a copy for snapshotting

            // 2. Apply it to your Package entity
            modelBuilder.Entity<Package>(entity =>
            {
                entity.Property(e => e.Inclusions)
                      .Metadata.SetValueComparer(stringListComparer);

                entity.Property(e => e.Tags)
                      .Metadata.SetValueComparer(stringListComparer);
            });
        }

        // Auto-update UpdatedAt on save
        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<User>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
                entry.Entity.UpdatedAt = DateTime.UtcNow;

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}