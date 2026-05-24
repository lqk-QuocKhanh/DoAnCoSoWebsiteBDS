using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.Type)
            .IsRequired();

        builder.Property(n => n.ReferenceId)
            .HasMaxLength(450);

        builder.Property(n => n.ActionUrl)
            .HasMaxLength(2000);

        builder.Property(n => n.RowVersion)
            .IsRowVersion();

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => new { n.UserId, n.IsRead });
        builder.HasIndex(n => n.Type);

        builder.HasQueryFilter(n => !n.IsDeleted);
    }
}
