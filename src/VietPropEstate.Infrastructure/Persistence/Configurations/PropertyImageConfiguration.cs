using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.ToTable("PropertyImages");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Url)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(pi => pi.Caption)
            .HasMaxLength(500);

        builder.Property(pi => pi.DisplayOrder)
            .HasDefaultValue(0);

        builder.Property(pi => pi.IsPrimary)
            .HasDefaultValue(false);

        builder.Property(pi => pi.RowVersion)
            .IsRowVersion();

        builder.HasQueryFilter(pi => !pi.IsDeleted);

        builder.HasIndex(pi => new { pi.PropertyId, pi.IsPrimary });
        builder.HasIndex(pi => pi.DisplayOrder);
    }
}
