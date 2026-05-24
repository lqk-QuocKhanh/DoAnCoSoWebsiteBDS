using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VietPropEstate.Infrastructure.Identity;

using VietPropEstate.Application.Common.Authorization;

namespace VietPropEstate.Infrastructure.Services;

/// <summary>
/// Seeds application roles and a default admin user on first startup.
/// Idempotent — safe to call multiple times.
/// </summary>
public sealed class RoleSeeder
{
    private static readonly string[] Roles = AppRoles.All;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RoleSeeder> _logger;

    public RoleSeeder(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<RoleSeeder> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var roleName in Roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    _logger.LogInformation("Created role: {Role}.", roleName);
                else
                    _logger.LogWarning("Failed to create role {Role}: {Errors}.",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        var adminSection = _configuration.GetSection("AdminSeed");
        var adminEmail = adminSection["Email"];
        var adminPassword = adminSection["Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            _logger.LogDebug("AdminSeed not configured — skipping default admin creation.");
            return;
        }

        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing is not null)
        {
            await EnsureAdminAccountAsync(userManager, existing, adminPassword);
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = adminSection["FirstName"] ?? "System",
            LastName = adminSection["LastName"] ?? "Admin",
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(admin, adminPassword);
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, AppRoles.SysAdmin);
            await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            _logger.LogInformation("Default SysAdmin user created: {Email}.", adminEmail);
        }
        else
        {
            _logger.LogWarning("Failed to create default admin: {Errors}.",
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
        }
    }

    private async Task EnsureAdminAccountAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser admin,
        string configuredPassword)
    {
        var changed = false;

        if (!admin.EmailConfirmed)
        {
            admin.EmailConfirmed = true;
            changed = true;
        }

        if (!admin.IsActive)
        {
            admin.IsActive = true;
            changed = true;
        }

        if (changed)
            await userManager.UpdateAsync(admin);

        if (!await userManager.IsInRoleAsync(admin, AppRoles.SysAdmin))
            await userManager.AddToRoleAsync(admin, AppRoles.SysAdmin);

        if (!await userManager.IsInRoleAsync(admin, AppRoles.Admin))
            await userManager.AddToRoleAsync(admin, AppRoles.Admin);

        var passwordValid = await userManager.CheckPasswordAsync(admin, configuredPassword);
        if (!passwordValid)
        {
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(admin);
            var resetResult = await userManager.ResetPasswordAsync(admin, resetToken, configuredPassword);
            if (resetResult.Succeeded)
                _logger.LogInformation("Admin password synchronized for {Email}.", admin.Email);
            else
                _logger.LogWarning("Failed to synchronize admin password for {Email}: {Errors}.",
                    admin.Email, string.Join(", ", resetResult.Errors.Select(e => e.Description)));
        }

        _logger.LogInformation("Admin account verified: {Email}, confirmed={Confirmed}, active={Active}.",
            admin.Email, admin.EmailConfirmed, admin.IsActive);
    }
}
