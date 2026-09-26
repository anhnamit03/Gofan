using GoFan.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Province)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.District)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Ward)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DetailedAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Note)
            .HasMaxLength(500);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}