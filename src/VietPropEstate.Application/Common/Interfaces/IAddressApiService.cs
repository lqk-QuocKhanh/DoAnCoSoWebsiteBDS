using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Common.Interfaces;

public interface IAddressApiService
{
    /// <summary>Fetches all provinces from the Vietnam address API (v2).</summary>
    Task<IReadOnlyList<Province>> GetProvincesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches all wards for a given province code from the Vietnam address API.
    /// Returns the wards with their ProvinceId resolved from the in-memory province list.
    /// </summary>
    Task<IReadOnlyList<Ward>> GetWardsAsync(int provinceCode, int provinceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Full sync: fetches all provinces and their wards from the API.
    /// Falls back to embedded JSON if the API is unavailable.
    /// </summary>
    Task<(IReadOnlyList<Province> Provinces, IReadOnlyList<Ward> Wards)> SyncVietnamAddressDataAsync(
        CancellationToken cancellationToken = default);
}
