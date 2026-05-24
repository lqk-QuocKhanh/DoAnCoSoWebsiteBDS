namespace VietPropEstate.Infrastructure.Persistence.Configurations;

internal static class PostgresIndexFilters
{
    public const string NotDeleted = "\"IsDeleted\" = false";
}
