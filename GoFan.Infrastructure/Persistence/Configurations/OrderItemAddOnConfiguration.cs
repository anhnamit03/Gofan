using GoFan.Domain.Orders;
using GoFan.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class OrderItemAddOnConfiguration : IEntityTypeConfiguration<OrderItemAddOn>
{
    public void Configure(EntityTypeBuilder<OrderItemAddOn> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AddOnProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.SKU)
            .HasMaxLength(50);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalPrice)
            .HasPrecision(18, 2);

        builder.HasOne<OrderItem>()
            .WithMany()
            .HasForeignKey(x => x.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.AddOnProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}