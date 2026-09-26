using GoFan.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class SystemConfigurationConfiguration
    : IEntityTypeConfiguration<SystemConfiguration>
{
    public void Configure(EntityTypeBuilder<SystemConfiguration> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Key)
            .IsUnique();

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Description)
            .HasMaxLength(500);
    }
}