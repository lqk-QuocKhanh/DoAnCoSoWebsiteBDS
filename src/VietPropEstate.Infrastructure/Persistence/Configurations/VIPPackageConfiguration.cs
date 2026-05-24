using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class VIPPackageConfiguration : IEntityTypeConfiguration<VIPPackage>
{
    // Stable seed GUIDs
    private static readonly Guid BasicId    = new("60000000-0000-0000-0000-000000000001");
    private static readonly Guid StandardId = new("60000000-0000-0000-0000-000000000002");
    private static readonly Guid PremiumId  = new("60000000-0000-0000-0000-000000000003");
    private static readonly Guid GoldId     = new("60000000-0000-0000-0000-000000000004");

    private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<VIPPackage> builder)
    {
        builder.ToTable("VIPPackages");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Description)
            .HasMaxLength(2000);

        builder.Property(v => v.DurationDays)
            .IsRequired();

        builder.Property(v => v.MaxListings)
            .IsRequired();

        builder.OwnsOne(v => v.Price, price =>
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

        builder.Property(v => v.RowVersion)
            .IsRowVersion();

        builder.HasIndex(v => v.Name).IsUnique();
        builder.HasIndex(v => v.IsActive);

        builder.HasQueryFilter(v => !v.IsDeleted);

        // ── Seed data ────────────────────────────────────────────────────────────
        builder.HasData(
            new
            {
                Id = BasicId, Name = "Cơ Bản", IsActive = true, IsDeleted = false,
                DurationDays = 30, MaxListings = 3,
                Description = "Đăng tối đa 3 tin rao trong 30 ngày. Phù hợp cho cá nhân.",
                CreatedAt = SeedDate, RowVersion = Array.Empty<byte>()
            },
            new
            {
                Id = StandardId, Name = "Tiêu Chuẩn", IsActive = true, IsDeleted = false,
                DurationDays = 30, MaxListings = 10,
                Description = "Đăng tối đa 10 tin rao trong 30 ngày. Phù hợp cho môi giới cá nhân.",
                CreatedAt = SeedDate, RowVersion = Array.Empty<byte>()
            },
            new
            {
                Id = PremiumId, Name = "Cao Cấp", IsActive = true, IsDeleted = false,
                DurationDays = 90, MaxListings = 30,
                Description = "Đăng tối đa 30 tin rao trong 90 ngày. Ưu tiên hiển thị trên trang chủ.",
                CreatedAt = SeedDate, RowVersion = Array.Empty<byte>()
            },
            new
            {
                Id = GoldId, Name = "Gold", IsActive = true, IsDeleted = false,
                DurationDays = 180, MaxListings = 99,
                Description = "Không giới hạn tin rao trong 180 ngày. Được gắn nhãn VIP nổi bật.",
                CreatedAt = SeedDate, RowVersion = Array.Empty<byte>()
            }
        );

        // Seed prices separately (owned entity seed)
        builder.OwnsOne(v => v.Price).HasData(
            new { VIPPackageId = BasicId,    Amount = 500_000m,   Currency = "VND" },
            new { VIPPackageId = StandardId, Amount = 1_000_000m, Currency = "VND" },
            new { VIPPackageId = PremiumId,  Amount = 2_500_000m, Currency = "VND" },
            new { VIPPackageId = GoldId,     Amount = 5_000_000m, Currency = "VND" }
        );
    }
}
