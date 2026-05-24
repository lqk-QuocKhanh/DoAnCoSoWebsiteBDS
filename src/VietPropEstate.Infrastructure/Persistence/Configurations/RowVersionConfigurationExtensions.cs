using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

internal static class RowVersionConfigurationExtensions
{
    /// <summary>
    /// PostgreSQL-compatible optimistic concurrency token.
    /// SQL Server rowversion auto-generates on insert; PostgreSQL bytea requires an explicit default.
    /// </summary>
    public static PropertyBuilder<byte[]> ConfigurePostgresRowVersion(this PropertyBuilder<byte[]> property) =>
        property
            .IsConcurrencyToken()
            .HasColumnType("bytea")
            .HasDefaultValue(Array.Empty<byte>());
}
