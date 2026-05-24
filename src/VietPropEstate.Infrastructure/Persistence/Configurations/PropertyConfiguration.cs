using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.ValueObjects;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(600);

        builder.HasIndex(p => p.Slug).IsUnique();

        builder.Property(p => p.Description)
            .HasMaxLength(5000);

        builder.Property(p => p.Area)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.ListingType)
            .IsRequired();

        builder.Property(p => p.ViewCount)
            .HasDefaultValue(0);

        builder.Property(p => p.IsFeatured)
            .HasDefaultValue(false);

        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("PriceAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            price.Property(m => m.Currency)
                .HasColumnName("PriceCurrency")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(500)
                .IsRequired();

            address.Property(a => a.Ward)
                .HasColumnName("AddressWard")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.District)
                .HasColumnName("AddressDistrict")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.Province)
                .HasColumnName("AddressProvince")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("AddressCountry")
                .HasMaxLength(100);

            // Renamed to avoid conflict with top-level Latitude/Longitude columns
            address.Property(a => a.Latitude)
                .HasColumnName("AddressLatitude");

            address.Property(a => a.Longitude)
                .HasColumnName("AddressLongitude");
        });

        // Vietnam structured address fields (post-2025 administrative model)
        builder.Property(p => p.ProvinceCode);

        builder.Property(p => p.ProvinceName)
            .HasMaxLength(200);

        builder.Property(p => p.WardCode);

        builder.Property(p => p.WardName)
            .HasMaxLength(200);

        builder.Property(p => p.FullAddress)
            .HasMaxLength(1000);

        builder.Property(p => p.Latitude);
        builder.Property(p => p.Longitude);

        builder.HasOne(p => p.Province)
            .WithMany()
            .HasForeignKey(p => p.ProvinceCode)
            .HasPrincipalKey(pr => pr.Code)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Ward)
            .WithMany()
            .HasForeignKey(p => p.WardCode)
            .HasPrincipalKey(w => w.Code)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.PropertyType)
            .WithMany(pt => pt.Properties)
            .HasForeignKey(p => p.PropertyTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Agent)
            .WithMany(a => a.Properties)
            .HasForeignKey(p => p.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.TransactionType)
            .WithMany(tt => tt.Properties)
            .HasForeignKey(p => p.TransactionTypeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.Images)
            .WithOne(i => i.Property)
            .HasForeignKey(i => i.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Favorites)
            .WithOne(f => f.Property)
            .HasForeignKey(f => f.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.PropertyViews)
            .WithOne(pv => pv.Property)
            .HasForeignKey(pv => pv.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.RowVersion)
            .ConfigurePostgresRowVersion();

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.ListingType);
        builder.HasIndex(p => p.AgentId);
        builder.HasIndex(p => p.PropertyTypeId);
        builder.HasIndex(p => p.TransactionTypeId);
        builder.HasIndex(p => p.IsFeatured);
        builder.HasIndex(p => p.PublishedAt);
        // Vietnam address indexes
        builder.HasIndex(p => p.ProvinceCode);
        builder.HasIndex(p => p.WardCode);
        // Composite for price range search within province
        builder.HasIndex(p => new { p.ProvinceCode, p.Status });
        builder.HasIndex(p => p.CreatedAt);
    }
}
