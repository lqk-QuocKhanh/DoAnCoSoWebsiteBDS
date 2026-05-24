using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Services;

/// <summary>
/// Fetches Vietnam administrative address data from the Open API v2.
/// Base URL: https://provinces.open-api.vn/api/v2/
/// Reflects the post-2025 administrative merger (Province → Ward, no District level).
/// </summary>
public sealed class AddressApiService : IAddressApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AddressApiService> _logger;

    // Relative paths for the v2 endpoint
    private const string ProvincesPath = "p";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public AddressApiService(HttpClient httpClient, ILogger<AddressApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Province>> GetProvincesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching provinces from Vietnam Address API...");

            var response = await _httpClient.GetAsync(ProvincesPath, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var dtos = JsonSerializer.Deserialize<List<ProvinceDto>>(json, JsonOptions) ?? [];

            var provinces = dtos
                .Where(d => d.Code > 0)
                .Select(d => Province.Create(d.Code, d.Name, d.Codename, d.DivisionType, d.PhoneCode))
                .ToList();

            _logger.LogInformation("Fetched {Count} provinces from API.", provinces.Count);
            return provinces;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch provinces from Vietnam Address API.");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Ward>> GetWardsAsync(
        int provinceCode,
        int provinceId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // depth=2 returns province + its wards
            var response = await _httpClient.GetAsync($"{ProvincesPath}/{provinceCode}?depth=2", cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var dto = JsonSerializer.Deserialize<ProvinceWithWardsDto>(json, JsonOptions);
            if (dto?.Wards is null) return [];

            var wards = dto.Wards
                .Where(w => w.Code > 0)
                .Select(w => Ward.Create(w.Code, w.Name, w.Codename, w.DivisionType, provinceCode, provinceId))
                .ToList();

            return wards;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch wards for province {Code}.", provinceCode);
            return [];
        }
    }

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<Province> Provinces, IReadOnlyList<Ward> Wards)> SyncVietnamAddressDataAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await FetchFromApiAsync(cancellationToken);
        }
        catch (Exception apiEx)
        {
            _logger.LogWarning(apiEx, "API unavailable. Attempting fallback from embedded JSON file.");

            try
            {
                return await LoadFromFallbackJsonAsync(cancellationToken);
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "Fallback JSON also failed. Address sync aborted.");
                throw new InvalidOperationException(
                    "Unable to load Vietnam address data: both the API and the local fallback failed.", fallbackEx);
            }
        }
    }

    // ─── Private helpers ────────────────────────────────────────────────────────

    private async Task<(IReadOnlyList<Province>, IReadOnlyList<Ward>)> FetchFromApiAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting full Vietnam address sync from API...");

        // 1. Fetch all provinces (flat list, no wards at this depth)
        var provinceResponse = await _httpClient.GetAsync(ProvincesPath, cancellationToken);
        provinceResponse.EnsureSuccessStatusCode();

        var provinceJson = await provinceResponse.Content.ReadAsStringAsync(cancellationToken);
        var provinceDtos = JsonSerializer.Deserialize<List<ProvinceDto>>(provinceJson, JsonOptions) ?? [];

        var provinces = provinceDtos
            .Where(d => d.Code > 0)
            .Select(d => Province.Create(d.Code, d.Name, d.Codename, d.DivisionType, d.PhoneCode))
            .ToList();

        _logger.LogInformation("Fetched {Count} provinces.", provinces.Count);

        // 2. Fetch wards per province (depth=2)
        var allWards = new List<Ward>();
        var provinceCodeToId = new Dictionary<int, int>(); // Code → temp index (no DB IDs yet)

        // We batch province fetches with a small delay to be polite to the API
        for (var i = 0; i < provinces.Count; i++)
        {
            if (cancellationToken.IsCancellationRequested) break;

            var province = provinces[i];

            try
            {
                var wardResponse = await _httpClient
                    .GetAsync($"{ProvincesPath}/{province.Code}?depth=2", cancellationToken);

                if (!wardResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Could not fetch wards for province {Code} ({Name}): {Status}",
                        province.Code, province.Name, wardResponse.StatusCode);
                    continue;
                }

                var wardJson = await wardResponse.Content.ReadAsStringAsync(cancellationToken);
                var provinceDetail = JsonSerializer.Deserialize<ProvinceWithWardsDto>(wardJson, JsonOptions);

                if (provinceDetail?.Wards is not null)
                {
                    // ProvinceId will be set after DB insert; use index as placeholder
                    var wards = provinceDetail.Wards
                        .Where(w => w.Code > 0)
                        .Select(w => Ward.Create(w.Code, w.Name, w.Codename, w.DivisionType, province.Code, 0))
                        .ToList();

                    allWards.AddRange(wards);
                    _logger.LogDebug("Province {Name}: {Count} wards.", province.Name, wards.Count);
                }

                // Throttle API calls slightly
                if (i < provinces.Count - 1)
                    await Task.Delay(50, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error fetching wards for province {Code}.", province.Code);
            }
        }

        _logger.LogInformation("Address sync complete. {Provinces} provinces, {Wards} wards.",
            provinces.Count, allWards.Count);

        return (provinces, allWards);
    }

    private static async Task<(IReadOnlyList<Province>, IReadOnlyList<Ward>)> LoadFromFallbackJsonAsync(
        CancellationToken cancellationToken)
    {
        var baseDir = AppContext.BaseDirectory;
        var jsonPath = Path.Combine(baseDir, "Data", "vietnam-addresses-v2.json");

        if (!File.Exists(jsonPath))
            throw new FileNotFoundException("Fallback address JSON not found.", jsonPath);

        await using var stream = File.OpenRead(jsonPath);
        var root = await JsonSerializer.DeserializeAsync<FallbackRoot>(stream, JsonOptions, cancellationToken)
            ?? throw new InvalidDataException("Fallback JSON is empty or malformed.");

        var provinces = root.Provinces
            .Select(p => Province.Create(p.Code, p.Name, p.Codename, p.DivisionType, p.PhoneCode))
            .ToList();

        var wards = root.Wards
            .Select(w => Ward.Create(w.Code, w.Name, w.Codename, w.DivisionType, w.ProvinceCode, 0))
            .ToList();

        return (provinces, wards);
    }

    // ─── API DTO models ──────────────────────────────────────────────────────────

    private class ProvinceDto
    {
        [JsonPropertyName("code")] public int Code { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("codename")] public string Codename { get; set; } = string.Empty;
        [JsonPropertyName("division_type")] public string DivisionType { get; set; } = string.Empty;
        [JsonPropertyName("phone_code")] public int PhoneCode { get; set; }
    }

    private sealed class ProvinceWithWardsDto : ProvinceDto
    {
        [JsonPropertyName("wards")] public List<WardDto>? Wards { get; set; }
    }

    private sealed class WardDto
    {
        [JsonPropertyName("code")] public int Code { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("codename")] public string Codename { get; set; } = string.Empty;
        [JsonPropertyName("division_type")] public string DivisionType { get; set; } = string.Empty;
    }

    // ─── Fallback JSON models ────────────────────────────────────────────────────

    private sealed class FallbackRoot
    {
        [JsonPropertyName("provinces")] public List<FallbackProvince> Provinces { get; set; } = [];
        [JsonPropertyName("wards")] public List<FallbackWard> Wards { get; set; } = [];
    }

    private sealed class FallbackProvince
    {
        [JsonPropertyName("code")] public int Code { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("codename")] public string Codename { get; set; } = string.Empty;
        [JsonPropertyName("division_type")] public string DivisionType { get; set; } = string.Empty;
        [JsonPropertyName("phone_code")] public int PhoneCode { get; set; }
    }

    private sealed class FallbackWard
    {
        [JsonPropertyName("code")] public int Code { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("codename")] public string Codename { get; set; } = string.Empty;
        [JsonPropertyName("division_type")] public string DivisionType { get; set; } = string.Empty;
        [JsonPropertyName("province_code")] public int ProvinceCode { get; set; }
    }
}
