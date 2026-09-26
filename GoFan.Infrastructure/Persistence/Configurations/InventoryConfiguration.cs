using GoFan.Domain.Inventory;
using GoFan.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ProductId)
            .IsUnique();

        builder.HasOne<Product>()
            .WithOne()
            .HasForeignKey<Inventory>(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}