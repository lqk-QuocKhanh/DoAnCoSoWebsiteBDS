using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class UserConversationSettingConfiguration : IEntityTypeConfiguration<UserConversationSetting>
{
    public void Configure(EntityTypeBuilder<UserConversationSetting> builder)
    {
        builder.ToTable("UserConversationSettings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(s => s.RowVersion)
            .IsRowVersion();

        builder.HasOne(s => s.Conversation)
            .WithMany()
            .HasForeignKey(s => s.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.UserId, s.ConversationId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(s => new { s.UserId, s.IsPinned });

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
