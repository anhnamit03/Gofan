using GoFan.Domain.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class MessageMediaConfiguration : IEntityTypeConfiguration<MessageMedia>
{
    public void Configure(EntityTypeBuilder<MessageMedia> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}