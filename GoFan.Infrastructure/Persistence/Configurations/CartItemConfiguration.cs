using GoFan.Domain.Carts;
using GoFan.Domain.Combos;
using GoFan.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.HasOne<Cart>()
            .WithMany()
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Combo>()
            .WithMany()
            .HasForeignKey(x => x.ComboId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}