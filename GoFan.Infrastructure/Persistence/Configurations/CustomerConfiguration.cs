using GoFan.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.Phone)
            .IsUnique();

        builder.Property(x => x.Email)
            .HasMaxLength(255);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.FullName)
            .HasMaxLength(200);

        builder.Property(x => x.AvatarUrl)
            .HasMaxLength(1000);
    }
}