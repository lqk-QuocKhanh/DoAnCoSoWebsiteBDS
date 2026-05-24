using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.BuyerId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(c => c.SellerId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(c => c.Subject)
            .HasMaxLength(500);

        builder.Property(c => c.LastMessagePreview)
            .HasMaxLength(150);

        builder.Property(c => c.LastMessageAt);
        builder.Property(c => c.BuyerUnreadCount).HasDefaultValue(0);
        builder.Property(c => c.SellerUnreadCount).HasDefaultValue(0);

        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.HasOne(c => c.Property)
            .WithMany()
            .HasForeignKey(c => c.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.BuyerId);
        builder.HasIndex(c => c.SellerId);
        builder.HasIndex(c => c.LastMessageAt);
        builder.HasIndex(c => new { c.PropertyId, c.BuyerId, c.SellerId }).IsUnique();

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
