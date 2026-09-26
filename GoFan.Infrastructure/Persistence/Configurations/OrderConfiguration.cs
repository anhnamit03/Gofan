using GoFan.Domain.Customers;
using GoFan.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.ShippingProvince)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ShippingDistrict)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ShippingWard)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ShippingAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ShippingNote)
            .HasMaxLength(500);

        builder.Property(x => x.OrderNote)
            .HasMaxLength(500);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(1000);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}