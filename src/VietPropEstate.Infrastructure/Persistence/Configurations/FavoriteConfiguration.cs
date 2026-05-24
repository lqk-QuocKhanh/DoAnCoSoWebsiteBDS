using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("Favorites");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(f => f.Note)
            .HasMaxLength(1000);

        builder.Property(f => f.RowVersion)
            .IsRowVersion();

        // A user can only favorite a property once
        builder.HasIndex(f => new { f.UserId, f.PropertyId }).IsUnique();

        builder.HasQueryFilter(f => !f.IsDeleted);
    }
}
