using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class PropertyViewConfiguration : IEntityTypeConfiguration<PropertyView>
{
    public void Configure(EntityTypeBuilder<PropertyView> builder)
    {
        builder.ToTable("PropertyViews");

        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.UserId)
            .HasMaxLength(450);

        builder.Property(pv => pv.IpAddress)
            .HasMaxLength(45);

        builder.Property(pv => pv.UserAgent)
            .HasMaxLength(500);

        builder.Property(pv => pv.SessionId)
            .HasMaxLength(200);

        builder.Property(pv => pv.ViewedAt)
            .IsRequired();

        builder.HasIndex(pv => pv.PropertyId);
        builder.HasIndex(pv => pv.UserId);
        builder.HasIndex(pv => pv.ViewedAt);
    }
}
