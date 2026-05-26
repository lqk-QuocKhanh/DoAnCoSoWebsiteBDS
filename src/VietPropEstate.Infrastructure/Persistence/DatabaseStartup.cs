using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VietPropEstate.Infrastructure.Services;

namespace VietPropEstate.Infrastructure.Persistence;

public static class DatabaseStartup
{
    private const string IdentityUsersTable = "AspNetUsers";

    public static async Task InitializeAsync(
        IServiceProvider services,
        IHostEnvironment environment,
        CancellationToken cancellationToken = default)
    {
        var logger = services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseStartup");

        if (environment.IsEnvironment("Testing"))
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync(cancellationToken);
            logger.LogInformation("Testing database ensured.");
            return;
        }

        var configuration = services.GetRequiredService<IConfiguration>();
        if (configuration.GetValue("UseInMemoryDatabase", false))
        {
            await using var scope = services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync(cancellationToken);
            logger.LogInformation("In-memory development database ensured.");
            await RunSeedersAsync(services, logger, cancellationToken);
            logger.LogInformation("Startup completed");
            return;
        }

        var migrationApplied = await ApplyMigrationsAsync(services, logger, cancellationToken);
        if (!migrationApplied)
        {
            logger.LogWarning("Skipping seeders because migration did not complete successfully.");
            return;
        }

        await RunSeedersAsync(services, logger, cancellationToken);
        logger.LogInformation("Startup completed");
    }

    private static async Task<bool> ApplyMigrationsAsync(
        IServiceProvider services,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Applying migrations...");

        try
        {
            await using var scope = services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!await db.Database.CanConnectAsync(cancellationToken))
            {
                logger.LogError("Cannot connect to PostgreSQL. Check ConnectionStrings__DefaultConnection or DATABASE_URL.");
                return false;
            }

            var applied = (await db.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();
            var pending = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            logger.LogInformation(
                "Migration state — applied: {AppliedCount}, pending: {PendingCount}",
                applied.Count,
                pending.Count);

            if (pending.Count > 0)
                logger.LogInformation("Pending migrations: {Migrations}", string.Join(", ", pending));

            await db.Database.MigrateAsync(cancellationToken);

            if (!await IdentityTablesExistAsync(db, cancellationToken))
            {
                logger.LogWarning(
                    "Identity tables missing after migrate. Resetting migration history and re-applying.");

                await db.Database.ExecuteSqlRawAsync(
                    "DROP TABLE IF EXISTS \"__EFMigrationsHistory\" CASCADE;",
                    cancellationToken);

                await db.Database.MigrateAsync(cancellationToken);
            }

            if (!await IdentityTablesExistAsync(db, cancellationToken))
            {
                logger.LogError("AspNetUsers still missing after migration recovery.");
                return false;
            }

            logger.LogInformation("Database migrated successfully");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed");
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    private static async Task<bool> IdentityTablesExistAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var connection = db.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name = @tableName);
                """;

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@tableName";
            parameter.Value = IdentityUsersTable;
            command.Parameters.Add(parameter);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is bool exists && exists;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private static async Task RunSeedersAsync(
        IServiceProvider services,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Running seeders...");

        try
        {
            var roleSeeder = services.GetRequiredService<RoleSeeder>();
            await roleSeeder.SeedAsync(cancellationToken);
            logger.LogInformation("Role and admin seeding completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Role seeding failed");
            Console.WriteLine(ex.ToString());
        }

        try
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var addressSeedEnabled = configuration.GetValue("AddressSeed:Enabled", true);

            if (addressSeedEnabled)
            {
                var addressSeeder = services.GetRequiredService<AddressDataSeeder>();
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromMinutes(10));
                await addressSeeder.SeedAsync(cts.Token);
                logger.LogInformation("Address seeding completed");
            }
            else
            {
                logger.LogInformation("Address seeding disabled (AddressSeed:Enabled=false).");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Address seeding failed");
            Console.WriteLine(ex.ToString());
        }

        try
        {
            var testSeeder = services.GetRequiredService<TestDataSeeder>();
            await testSeeder.SeedAsync(cancellationToken);
            logger.LogInformation("Test data seeding completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Test data seeding failed");
            Console.WriteLine(ex.ToString());
        }
    }
}
