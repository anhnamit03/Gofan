using GoFan.Domain.Products;
using GoFan.Domain.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class ReviewMediaConfiguration : IEntityTypeConfiguration<ReviewMedia>
{
    public void Configure(EntityTypeBuilder<ReviewMedia> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne<Review>()
            .WithMany()
            .HasForeignKey(x => x.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}