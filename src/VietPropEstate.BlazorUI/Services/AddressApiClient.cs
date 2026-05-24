using System.Net.Http.Json;
using VietPropEstate.BlazorUI.Models;

namespace VietPropEstate.BlazorUI.Services;

public interface IAddressApiClient
{
    Task<List<AddressProvince>> GetProvincesAsync();
    Task<List<AddressWard>> GetWardsAsync(int provinceCode);
}

public sealed class AddressApiClient : IAddressApiClient
{
    private readonly HttpClient _http;
    private List<AddressProvince>? _cachedProvinces;
    private readonly Dictionary<int, List<AddressWard>> _wardCache = new();

    public AddressApiClient(HttpClient http) => _http = http;

    public async Task<List<AddressProvince>> GetProvincesAsync()
    {
        if (_cachedProvinces is not null)
            return _cachedProvinces;

        try
        {
            var result = await _http.GetFromJsonAsync<List<AddressProvince>>("api/addresses/provinces");
            _cachedProvinces = result ?? [];
        }
        catch
        {
            _cachedProvinces = [];
        }

        return _cachedProvinces;
    }

    public async Task<List<AddressWard>> GetWardsAsync(int provinceCode)
    {
        if (_wardCache.TryGetValue(provinceCode, out var cached))
            return cached;

        try
        {
            var result = await _http.GetFromJsonAsync<List<AddressWard>>($"api/addresses/wards/{provinceCode}");
            var wards = result ?? [];
            _wardCache[provinceCode] = wards;
            return wards;
        }
        catch
        {
            return [];
        }
    }
}
