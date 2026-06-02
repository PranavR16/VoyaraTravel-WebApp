using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Core;
using System.Text.Json;
using Voyara.Core.Entities;
namespace Voyara.Infrastructure;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        builder.Property(p => p.OldPrice)
            .HasColumnType("decimal(12,2)");

        builder.Property(p => p.Category)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Badge)
            .HasMaxLength(50);

        builder.Property(p => p.BadgeClass)
            .HasMaxLength(50);

        builder.Property(p => p.Image)
            .HasMaxLength(500);

        builder.Property(p => p.Unit)
            .HasMaxLength(50)
            .HasDefaultValue("per person");

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Store List<string> as JSON in SQL Server nvarchar(max)
        builder.Property(p => p.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v,
                    (JsonSerializerOptions?)null) ?? new List<string>()
            )
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.Inclusions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v,
                    (JsonSerializerOptions?)null) ?? new List<string>()
            )
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(p => p.Destination)
            .WithMany(d => d.Packages)
            .HasForeignKey(p => p.DestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Bookings)
            .WithOne(b => b.Package)
            .HasForeignKey(b => b.PackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.ReviewList)
            .WithOne(r => r.Package)
            .HasForeignKey(r => r.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Wishlists)
            .WithOne(w => w.Package)
            .HasForeignKey(w => w.PackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}