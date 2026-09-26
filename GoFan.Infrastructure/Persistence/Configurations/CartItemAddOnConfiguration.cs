using GoFan.Domain.Carts;
using GoFan.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class CartItemAddOnConfiguration : IEntityTypeConfiguration<CartItemAddOn>
{
    public void Configure(EntityTypeBuilder<CartItemAddOn> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.HasOne<CartItem>()
            .WithMany()
            .HasForeignKey(x => x.CartItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.AddOnProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}