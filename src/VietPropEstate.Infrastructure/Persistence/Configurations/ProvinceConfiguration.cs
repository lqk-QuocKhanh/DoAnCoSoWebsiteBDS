using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable("Provinces");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .UseIdentityColumn();

        builder.Property(p => p.Code)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Codename)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.DivisionType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PhoneCode)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.HasMany(p => p.Wards)
            .WithOne(w => w.Province)
            .HasForeignKey(w => w.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Province.Code is unique and used as a lookup key
        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasIndex(p => p.Codename);
        builder.HasIndex(p => p.IsActive);
    }
}
