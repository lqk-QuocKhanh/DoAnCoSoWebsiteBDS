using System.Net.Http.Json;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Payments.DTOs;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.Services;

public interface IPaymentApiClient
{
    Task<List<VIPPackageDto>> GetVipPackagesAsync();
    Task<List<UserVIPPackageDto>> GetMySubscriptionsAsync();
    Task<(PaymentInitiationDto? Result, string? ErrorMessage)> InitiatePaymentAsync(Guid vipPackageId);
    Task<PaginatedList<PaymentDto>?> GetPaymentHistoryAsync(int page = 1, PaymentStatus? status = null);
}

public sealed class PaymentApiClient : IPaymentApiClient
{
    private readonly HttpClient _http;

    public PaymentApiClient(HttpClient http) => _http = http;

    public async Task<List<VIPPackageDto>> GetVipPackagesAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/vip-packages");
            if (!response.IsSuccessStatusCode)
                return [];

            return await response.Content.ReadFromJsonAsync<List<VIPPackageDto>>() ?? [];
        }
        catch { return []; }
    }

    public async Task<List<UserVIPPackageDto>> GetMySubscriptionsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<UserVIPPackageDto>>("api/vip-packages/my-subscriptions") ?? [];
        }
        catch { return []; }
    }

    public async Task<(PaymentInitiationDto? Result, string? ErrorMessage)> InitiatePaymentAsync(Guid vipPackageId)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/payments/initiate",
                new { VIPPackageId = vipPackageId, Locale = "vn" });
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                var message = TryReadErrorMessage(body) ?? "Không thể khởi tạo thanh toán.";
                return (null, message);
            }

            var result = await resp.Content.ReadFromJsonAsync<PaymentInitiationDto>();
            return (result, null);
        }
        catch
        {
            return (null, "Không thể kết nối máy chủ thanh toán.");
        }
    }

    private static string? TryReadErrorMessage(string body)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.TryGetProperty("message", out var messageProp))
                return messageProp.GetString();
            if (root.TryGetProperty("errors", out var errorsProp) &&
                errorsProp.ValueKind == System.Text.Json.JsonValueKind.Object &&
                errorsProp.TryGetProperty("message", out var nested))
                return nested.GetString();
        }
        catch { }
        return null;
    }

    public async Task<PaginatedList<PaymentDto>?> GetPaymentHistoryAsync(int page = 1, PaymentStatus? status = null)
    {
        try
        {
            var url = $"api/payments?pageNumber={page}&pageSize=20";
            if (status.HasValue) url += $"&status={(int)status.Value}";
            return await _http.GetFromJsonAsync<PaginatedList<PaymentDto>>(url);
        }
        catch { return null; }
    }
}
