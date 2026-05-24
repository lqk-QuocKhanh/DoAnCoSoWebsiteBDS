using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionType>
{
    private static readonly Guid SellId = new("44444444-4444-4444-4444-444444444444");
    private static readonly Guid RentId = new("55555555-5555-5555-5555-555555555555");

    private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<TransactionType> builder)
    {
        builder.ToTable("TransactionTypes");

        builder.HasKey(tt => tt.Id);

        builder.Property(tt => tt.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(tt => tt.Description)
            .HasMaxLength(1000);

        builder.Property(tt => tt.RowVersion)
            .IsRowVersion();

        builder.HasIndex(tt => tt.Name).IsUnique();

        builder.HasQueryFilter(tt => !tt.IsDeleted);

        builder.HasData(
            new
            {
                Id = SellId, Name = "Sell", Description = "Property is listed for sale",
                IsActive = true, IsDeleted = false, CreatedAt = SeedDate,
                RowVersion = Array.Empty<byte>()
            },
            new
            {
                Id = RentId, Name = "Rent", Description = "Property is listed for rent",
                IsActive = true, IsDeleted = false, CreatedAt = SeedDate,
                RowVersion = Array.Empty<byte>()
            }
        );
    }
}
