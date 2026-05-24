using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId)
            .HasMaxLength(450);

        builder.Property(a => a.EntityName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.EntityId)
            .HasMaxLength(450);

        // JSON columns — store as nvarchar(max) / text
        builder.Property(a => a.OldValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.NewValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.AffectedColumns)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.Timestamp)
            .IsRequired();

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Action);
        builder.HasIndex(a => new { a.EntityName, a.EntityId });
        builder.HasIndex(a => a.Timestamp);
    }
}
