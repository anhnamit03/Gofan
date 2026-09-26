using GoFan.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class ProductAddOnConfiguration : IEntityTypeConfiguration<ProductAddOn>
{
    public void Configure(EntityTypeBuilder<ProductAddOn> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.MainProductId,
            x.AddOnProductId
        }).IsUnique();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.MainProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.AddOnProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}