using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Voyara.Core;
using Voyara.Core.Entities;
namespace Voyara.Infrastructure.Data.Configurations
{
    public class BookingAddonConfiguration : IEntityTypeConfiguration<BookingAddon>
    {
        public void Configure(EntityTypeBuilder<BookingAddon> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Price)
                .HasColumnType("decimal(12,2)");
        }
    }
}
