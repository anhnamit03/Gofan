using GoFan.Domain.Carts;
using GoFan.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.CustomerId)
            .IsUnique();

        builder.HasOne<Customer>()
            .WithOne()
            .HasForeignKey<Cart>(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}