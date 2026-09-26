using GoFan.Domain.Products;
using GoFan.Domain.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class ProductPromotionConfiguration : IEntityTypeConfiguration<ProductPromotion>
{
    public void Configure(EntityTypeBuilder<ProductPromotion> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.ProductId,
            x.PromotionId
        }).IsUnique();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Promotion>()
            .WithMany()
            .HasForeignKey(x => x.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}