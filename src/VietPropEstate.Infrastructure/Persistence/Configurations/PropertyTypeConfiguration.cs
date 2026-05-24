using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class PropertyTypeConfiguration : IEntityTypeConfiguration<PropertyType>
{
    // Stable seed GUIDs
    private static readonly Guid ApartmentId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");
    private static readonly Guid HouseId      = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid VillaId      = new("B2C3D4E5-F6A7-8901-BCDE-F12345678901");
    private static readonly Guid LandId       = new("D4E5F6A7-B8C9-0123-DEF0-234567890123");
    private static readonly Guid OfficeId     = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid MotelId      = new("33333333-3333-3333-3333-333333333333");

    private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<PropertyType> builder)
    {
        builder.ToTable("PropertyTypes");

        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pt => pt.Description)
            .HasMaxLength(1000);

        builder.Property(pt => pt.RowVersion)
            .ConfigurePostgresRowVersion();

        builder.HasIndex(pt => pt.Name).IsUnique();

        builder.HasQueryFilter(pt => !pt.IsDeleted);

        builder.HasData(
            new
            {
                Id = ApartmentId, Name = "Apartment", Description = "Condominium / Apartment unit",
                IsActive = true, IsDeleted = false, CreatedAt = SeedDate,
                RowVersion = Array.Empty<byte>()
            },
            new
            {
                Id = HouseId, Name = "House", Description = "Detached or semi-detached house",
                IsActive = true, IsDeleted = false, CreatedAt = SeedDate,
                RowVersion = Array.Empty<byte>()
            },
            new
            {
                Id = VillaId, Name = "Villa", Description = "Standalone villa with garden",
                IsActive = true, IsDeleted = false, CreatedAt = SeedDate,
                RowVersion = Array.Empty<byte>()
            },
            new
            {
                Id = LandId, Name = "Land", Description = "Land plot / undeveloped lot",
                IsActive = true, IsDeleted = false, CreatedAt = SeedDate,
                RowVersion = Array.Empty<byte>()
            },
            new
            {
                Id = OfficeId, Name = "Office", Description = "Commercial office space",
                IsActive = true, IsDeleted = false, CreatedAt = SeedDate,
                RowVersion = Array.Empty<byte>()
            },
            new
            {
                Id = MotelId, Name = "Motel", Description = "Motel / mini-hotel / boarding house",
                IsActive = true, IsDeleted = false, CreatedAt = SeedDate,
                RowVersion = Array.Empty<byte>()
            }
        );
    }
}
