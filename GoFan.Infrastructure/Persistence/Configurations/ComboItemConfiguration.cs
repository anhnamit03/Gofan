using GoFan.Domain.Combos;
using GoFan.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class ComboItemConfiguration : IEntityTypeConfiguration<ComboItem>
{
    public void Configure(EntityTypeBuilder<ComboItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.ComboId,
            x.ProductId
        }).IsUnique();

        builder.HasOne<Combo>()
            .WithMany()
            .HasForeignKey(x => x.ComboId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}