using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.SenderId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(m => m.AttachmentUrl)
            .HasMaxLength(2000);

        builder.Property(m => m.Status)
            .IsRequired();

        builder.Property(m => m.RowVersion)
            .ConfigurePostgresRowVersion();

        builder.HasIndex(m => m.ConversationId);
        builder.HasIndex(m => m.SenderId);
        builder.HasIndex(m => m.Status);

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
