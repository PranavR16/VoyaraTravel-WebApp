using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Voyara.Core;
using Voyara.Core.Entities;
namespace Voyara.Infrastructure.Data.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasKey(w => w.Id);

            // One user can only wishlist a package once
            builder.HasIndex(w => new { w.UserId, w.PackageId })
                .IsUnique();

            builder.Property(w => w.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
