using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class UserBlockConfiguration : IEntityTypeConfiguration<UserBlock>
{
    public void Configure(EntityTypeBuilder<UserBlock> builder)
    {
        builder.ToTable("UserBlocks");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BlockerId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(b => b.BlockedUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(b => b.RowVersion)
            .ConfigurePostgresRowVersion();

        builder.HasIndex(b => new { b.BlockerId, b.BlockedUserId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(b => b.BlockedUserId);

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
