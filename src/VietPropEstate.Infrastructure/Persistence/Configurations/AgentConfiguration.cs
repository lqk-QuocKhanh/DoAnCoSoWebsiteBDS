using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable("Agents");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FullName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.LicenseNumber)
            .HasMaxLength(100);

        builder.Property(a => a.AgencyName)
            .HasMaxLength(300);

        builder.Property(a => a.AvatarUrl)
            .HasMaxLength(2000);

        builder.Property(a => a.Bio)
            .HasMaxLength(3000);

        builder.Property(a => a.RowVersion)
            .IsRowVersion();

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.HasIndex(a => a.Email).IsUnique();
        builder.HasIndex(a => a.UserId);
    }
}
