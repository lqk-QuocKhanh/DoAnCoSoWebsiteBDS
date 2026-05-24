using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VietPropEstate.Infrastructure.Services;

namespace VietPropEstate.Infrastructure.Persistence;

public static class DatabaseStartup
{
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

            var pending = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
            if (pending.Count > 0)
                logger.LogInformation("Pending migrations: {Migrations}", string.Join(", ", pending));
            else
                logger.LogInformation("No pending migrations detected.");

            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await db.Database.MigrateAsync(cancellationToken);
            });

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
            var addressSeeder = services.GetRequiredService<AddressDataSeeder>();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromMinutes(10));
            await addressSeeder.SeedAsync(cts.Token);
            logger.LogInformation("Address seeding completed");
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
