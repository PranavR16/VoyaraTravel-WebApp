using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Voyara.Core.Entities;

namespace Voyara.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.RazorpayOrderId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.RazorpayPaymentId)
                .HasMaxLength(100);

            builder.Property(p => p.RazorpaySignature)
                .HasMaxLength(200);

            builder.Property(p => p.Method)
                .HasMaxLength(30);

            builder.Property(p => p.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(12,2)");

            builder.Property(p => p.Currency)
                .HasMaxLength(5)
                .HasDefaultValue("INR");

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
