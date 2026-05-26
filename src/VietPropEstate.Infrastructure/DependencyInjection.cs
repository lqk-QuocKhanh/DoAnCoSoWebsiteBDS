using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Interfaces;
using VietPropEstate.Infrastructure.Caching;
using VietPropEstate.Infrastructure.Identity;
using VietPropEstate.Infrastructure.Persistence;
using VietPropEstate.Infrastructure.Persistence.Repositories;
using VietPropEstate.Infrastructure.Services;

namespace VietPropEstate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment? environment = null)
    {
        // ── Database ────────────────────────────────────────────────────────────
        var connectionString = DatabaseConnectionResolver.Resolve(configuration);
        var useInMemoryDatabase = environment?.IsEnvironment("Testing") == true
            || configuration.GetValue("UseInMemoryDatabase", false)
            || (environment?.IsDevelopment() == true && string.IsNullOrWhiteSpace(connectionString));

        if (useInMemoryDatabase)
        {
            var databaseName = configuration["InMemoryDatabaseName"] ?? "VietPropEstateTests";
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName)
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        }
        else
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "PostgreSQL connection string is missing. On Render set ConnectionStrings__DefaultConnection " +
                    "to your Render Postgres URL (postgresql://...) or Npgsql format (Host=...;Database=...). " +
                    "Alternatively link DATABASE_URL from your Render PostgreSQL instance.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(
                            typeof(ApplicationDbContext).Assembly.GetName().Name);
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null);
                        npgsqlOptions.CommandTimeout(60);
                    }));
        }

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── Redis distributed cache ─────────────────────────────────────────────
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "VietPropEstate:";
            });
        }
        else
        {
            // Fall back to in-memory cache when Redis is not configured
            services.AddDistributedMemoryCache();
        }

        services.AddScoped<ICacheService, RedisCacheService>();

        // ── Vietnam Address API ─────────────────────────────────────────────────
        services.AddHttpClient<IAddressApiService, AddressApiService>(client =>
            {
                client.BaseAddress = new Uri("https://provinces.open-api.vn/api/v2/");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "VietPropEstate/1.0");
            })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromSeconds(2);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(120);
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
            });

        // ── Address data seeder ─────────────────────────────────────────────────
        services.AddSingleton<AddressDataSeeder>();

        // ── Role seeder ─────────────────────────────────────────────────────────
        services.AddSingleton<RoleSeeder>();
        services.AddSingleton<TestDataSeeder>();
        services.AddSingleton<ProductionDataSeeder>();

        // ── Auth services ───────────────────────────────────────────────────────
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<IUserAvatarService, UserAvatarService>();
        services.AddScoped<IUserPublicProfileService, UserPublicProfileService>();
        services.AddScoped<IUserPhoneVerificationService, UserPhoneVerificationService>();
        services.AddScoped<IIdentityUserLookup, IdentityUserLookup>();

        // ── Chat / real-time services ───────────────────────────────────────────
        services.AddSingleton<IOnlineUserTracker, InMemoryOnlineUserTracker>();
        services.AddScoped<INotificationService, NotificationService>();

        // ── Payment / VNPay ─────────────────────────────────────────────────────
        services.AddScoped<IVnPayService, VnPayService>();

        return services;
    }
}
