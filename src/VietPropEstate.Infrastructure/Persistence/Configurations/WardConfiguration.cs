using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class WardConfiguration : IEntityTypeConfiguration<Ward>
{
    public void Configure(EntityTypeBuilder<Ward> builder)
    {
        builder.ToTable("Wards");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .UseIdentityColumn();

        builder.Property(w => w.Code)
            .IsRequired();

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Codename)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.DivisionType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.ProvinceCode)
            .IsRequired();

        builder.Property(w => w.ProvinceId)
            .IsRequired();

        builder.Property(w => w.IsActive)
            .HasDefaultValue(true);

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.Property(w => w.UpdatedAt);

        builder.HasOne(w => w.Province)
            .WithMany(p => p.Wards)
            .HasForeignKey(w => w.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ward.Code is unique nationally
        builder.HasIndex(w => w.Code).IsUnique();
        builder.HasIndex(w => w.ProvinceCode);
        builder.HasIndex(w => w.Codename);
        builder.HasIndex(w => new { w.ProvinceId, w.IsActive });
    }
}
