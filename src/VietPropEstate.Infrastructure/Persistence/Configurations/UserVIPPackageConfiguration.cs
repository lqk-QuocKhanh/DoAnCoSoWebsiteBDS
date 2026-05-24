using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class UserVIPPackageConfiguration : IEntityTypeConfiguration<UserVIPPackage>
{
    public void Configure(EntityTypeBuilder<UserVIPPackage> builder)
    {
        builder.ToTable("UserVIPPackages");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(u => u.StartDate)
            .IsRequired();

        builder.Property(u => u.EndDate)
            .IsRequired();

        builder.Property(u => u.RemainingListings)
            .IsRequired();

        builder.Property(u => u.RowVersion)
            .IsRowVersion();

        builder.HasOne(u => u.VIPPackage)
            .WithMany(v => v.UserPackages)
            .HasForeignKey(u => u.VIPPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Payments)
            .WithOne(p => p.UserVIPPackage)
            .HasForeignKey(p => p.UserVIPPackageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(u => u.UserId);
        builder.HasIndex(u => new { u.UserId, u.IsActive });
        builder.HasIndex(u => u.EndDate);

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
