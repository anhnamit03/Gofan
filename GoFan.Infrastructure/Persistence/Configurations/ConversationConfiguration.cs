using GoFan.Domain.Chat;
using GoFan.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFan.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.CustomerId)
            .IsUnique();

        builder.HasOne<Customer>()
            .WithOne()
            .HasForeignKey<Conversation>(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}