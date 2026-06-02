using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Voyara.Core;
using Voyara.Core.Entities;
namespace Voyara.Infrastructure.Data.Configurations
{
    public class BookingTravelerConfiguration : IEntityTypeConfiguration<BookingTraveler>
    {
        public void Configure(EntityTypeBuilder<BookingTraveler> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(t => t.Count)
                .IsRequired();
        }
    }
}
