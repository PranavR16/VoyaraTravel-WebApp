using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Voyara.Core;
using Voyara.Core.Entities;
namespace Voyara.Infrastructure.Data.Configurations
{
    public class DestinationConfiguration : IEntityTypeConfiguration<Destination>
    {
        public void Configure(EntityTypeBuilder<Destination> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Image)
                .HasMaxLength(500);

            builder.Property(d => d.StartingPrice)
                .HasColumnType("decimal(12,2)");

            builder.Property(d => d.Description)
                .HasMaxLength(1000);

            builder.Property(d => d.IsActive)
                .HasDefaultValue(true);

            builder.Property(d => d.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
