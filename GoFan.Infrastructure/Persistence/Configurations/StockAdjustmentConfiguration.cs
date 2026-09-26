using GoFan.Domain.Admins;
using GoFan.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne<Inventory>()
            .WithMany()
            .HasForeignKey(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Admin>()
            .WithMany()
            .HasForeignKey(x => x.AdminId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}