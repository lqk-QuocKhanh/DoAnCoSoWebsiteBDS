using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Infrastructure.Persistence;

namespace VietPropEstate.Infrastructure.Services;

/// <summary>
/// Seeds Vietnam province and ward data into the database on first startup.
/// Idempotent: will not import data if the Provinces table already has records.
/// Uses CreateExecutionStrategy to work correctly with EF retry execution strategy.
/// </summary>
public sealed class AddressDataSeeder
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AddressDataSeeder> _logger;

    private const int WardBatchSize = 500;

    public AddressDataSeeder(IServiceScopeFactory scopeFactory, ILogger<AddressDataSeeder> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var addressService = scope.ServiceProvider.GetRequiredService<IAddressApiService>();

        var alreadySeeded = await context.Provinces.AnyAsync(cancellationToken);
        if (alreadySeeded)
        {
            _logger.LogInformation("Vietnam address data already seeded. Skipping.");
            return;
        }

        _logger.LogInformation("Starting Vietnam address data seeding...");

        // EF retry execution strategy requires all operations inside CreateExecutionStrategy
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var (provinces, wards) = await addressService.SyncVietnamAddressDataAsync(cancellationToken);

                if (provinces.Count == 0)
                {
                    _logger.LogWarning("No provinces returned from address source. Seeding aborted.");
                    await transaction.RollbackAsync(CancellationToken.None);
                    return;
                }

                // 1. Insert provinces
                _logger.LogInformation("Inserting {Count} provinces...", provinces.Count);
                await context.Provinces.AddRangeAsync(provinces, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                // 2. Build code → DB Id map
                var provinceIdMap = await context.Provinces
                    .AsNoTracking()
                    .Select(p => new { p.Code, p.Id })
                    .ToDictionaryAsync(p => p.Code, p => p.Id, cancellationToken);

                // 3. Assign real ProvinceId to wards and insert in batches
                var wardList = wards.ToList();
                _logger.LogInformation("Inserting {Count} wards in batches of {Batch}...",
                    wardList.Count, WardBatchSize);

                var updatedWards = wardList
                    .Where(w => provinceIdMap.ContainsKey(w.ProvinceCode))
                    .Select(w => Ward.Create(
                        w.Code, w.Name, w.Codename, w.DivisionType,
                        w.ProvinceCode, provinceIdMap[w.ProvinceCode]))
                    .ToList();

                for (var i = 0; i < updatedWards.Count; i += WardBatchSize)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    var batch = updatedWards.Skip(i).Take(WardBatchSize).ToList();
                    await context.Wards.AddRangeAsync(batch, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);

                    _logger.LogDebug("Inserted wards {From}–{To} of {Total}.",
                        i + 1, Math.Min(i + WardBatchSize, updatedWards.Count), updatedWards.Count);
                }

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Vietnam address data seeded: {Provinces} provinces, {Wards} wards.",
                    provinces.Count, updatedWards.Count);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Address seeding was cancelled.");
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Address seeding failed. Rolling back transaction.");
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        });
    }
}
