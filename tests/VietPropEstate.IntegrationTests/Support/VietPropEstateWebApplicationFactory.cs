using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.IntegrationTests.Support;

public sealed class VietPropEstateWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"VietPropEstateTests-{Guid.NewGuid():N}";
    private bool _seeded;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("InMemoryDatabaseName", _databaseName);

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseInMemoryDatabase"] = "true",
                ["InMemoryDatabaseName"] = _databaseName,
                ["TestSeed:Enabled"] = "false",
                ["RateLimiting:PermitLimit"] = "100000",
                ["RateLimiting:WindowSeconds"] = "60",
                ["AdminSeed:Email"] = "admin@vietpropestate.vn",
                ["AdminSeed:Password"] = "Admin@123",
                ["AdminSeed:FirstName"] = "System",
                ["AdminSeed:LastName"] = "Admin"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IChatNotificationService>();
            services.AddSingleton<IChatNotificationService, NoOpChatNotificationService>();
        });
    }

    public async Task EnsureSeededAsync()
    {
        if (_seeded)
            return;

        using var scope = Services.CreateScope();
        await IntegrationDataSeeder.SeedAsync(scope.ServiceProvider);
        _seeded = true;
    }
}
