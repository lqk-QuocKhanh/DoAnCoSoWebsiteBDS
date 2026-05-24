using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        var useInMemoryDatabase = environment?.IsEnvironment("Testing") == true
            || configuration.GetValue("UseInMemoryDatabase", false);

        if (useInMemoryDatabase)
        {
            var databaseName = configuration["InMemoryDatabaseName"] ?? "VietPropEstateTests";
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                        sqlOptions.CommandTimeout(60);
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

        // ── Chat / real-time services ───────────────────────────────────────────
        services.AddSingleton<IOnlineUserTracker, InMemoryOnlineUserTracker>();
        services.AddScoped<INotificationService, NotificationService>();

        // ── Payment / VNPay ─────────────────────────────────────────────────────
        services.AddScoped<IVnPayService, VnPayService>();

        return services;
    }
}
