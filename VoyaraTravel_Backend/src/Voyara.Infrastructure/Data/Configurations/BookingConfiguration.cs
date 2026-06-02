using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Core;
using Voyara.Core.Entities;
namespace Voyara.Infrastructure;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BookingRef)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(b => b.BookingRef)
            .IsUnique();

        builder.Property(b => b.Status)
            .HasMaxLength(20)
            .HasDefaultValue("Pending");

        builder.Property(b => b.TotalAmount)
            .HasColumnType("decimal(12,2)");

        builder.Property(b => b.TaxAmount)
            .HasColumnType("decimal(12,2)");

        builder.Property(b => b.DiscountAmount)
            .HasColumnType("decimal(12,2)");

        builder.Property(b => b.CouponCode)
            .HasMaxLength(20);

        builder.Property(b => b.FlightClass)
            .HasMaxLength(20)
            .HasDefaultValue("economy");

        builder.Property(b => b.RoomType)
            .HasMaxLength(20)
            .HasDefaultValue("std");

        builder.Property(b => b.SpecialRequests)
            .HasMaxLength(1000);

        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // One-to-one with Payment
        builder.HasOne(b => b.Payment)
            .WithOne(p => p.Booking)
            .HasForeignKey<Payment>(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-many with Travelers
        builder.HasMany(b => b.Travelers)
            .WithOne(t => t.Booking)
            .HasForeignKey(t => t.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-many with Addons
        builder.HasMany(b => b.Addons)
            .WithOne(a => a.Booking)
            .HasForeignKey(a => a.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
