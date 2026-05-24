using System.Net.Http.Json;
using System.Web;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.Commands.CreateProperty;
using VietPropEstate.Application.Features.Properties.Commands.UpdateProperty;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.Services;

public class PropertyApiService : IPropertyApiService
{
    private readonly HttpClient _httpClient;
    private readonly IMediaUrlResolver _mediaUrls;

    public PropertyApiService(HttpClient httpClient, IMediaUrlResolver mediaUrls)
    {
        _httpClient = httpClient;
        _mediaUrls = mediaUrls;
    }

    public async Task<PaginatedList<PropertyDto>?> GetPropertiesAsync(
        int pageNumber = 1, int pageSize = 12,
        string? searchTerm = null,
        ListingType? listingType = null,
        string? province = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int? provinceCode = null,
        int? wardCode = null,
        Guid? propertyTypeId = null,
        Guid? transactionTypeId = null,
        decimal? minArea = null,
        decimal? maxArea = null,
        int? minBedrooms = null,
        bool? isFeatured = null,
        PropertyStatus? status = PropertyStatus.Active,
        string sortBy = "CreatedAt",
        string sortOrder = "Desc",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["pageNumber"] = pageNumber.ToString();
            query["pageSize"] = pageSize.ToString();
            if (!string.IsNullOrWhiteSpace(searchTerm)) query["searchTerm"] = searchTerm;
            if (listingType.HasValue) query["listingType"] = ((int)listingType.Value).ToString();
            if (!string.IsNullOrWhiteSpace(province)) query["provinceName"] = province;
            if (minPrice.HasValue) query["minPrice"] = minPrice.Value.ToString();
            if (maxPrice.HasValue) query["maxPrice"] = maxPrice.Value.ToString();
            if (provinceCode.HasValue) query["provinceCode"] = provinceCode.Value.ToString();
            if (wardCode.HasValue) query["wardCode"] = wardCode.Value.ToString();
            if (propertyTypeId.HasValue) query["propertyTypeId"] = propertyTypeId.Value.ToString();
            if (transactionTypeId.HasValue) query["transactionTypeId"] = transactionTypeId.Value.ToString();
            if (minArea.HasValue) query["minArea"] = minArea.Value.ToString();
            if (maxArea.HasValue) query["maxArea"] = maxArea.Value.ToString();
            if (minBedrooms.HasValue) query["minBedrooms"] = minBedrooms.Value.ToString();
            if (isFeatured.HasValue) query["isFeatured"] = isFeatured.Value.ToString();
            if (status.HasValue) query["status"] = ((int)status.Value).ToString();
            else query["status"] = ((int)PropertyStatus.Active).ToString();
            query["sortBy"] = sortBy;
            query["sortOrder"] = sortOrder;

            return await _httpClient.GetFromJsonAsync<PaginatedList<PropertyDto>>(
                $"api/properties?{query}", cancellationToken) is { } result
                ? NormalizeList(result)
                : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<PropertyDetailDto?> GetPropertyByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var detail = await _httpClient.GetFromJsonAsync<PropertyDetailDto>(
                $"api/properties/{id}", cancellationToken);
            return NormalizeDetail(detail);
        }
        catch
        {
            return null;
        }
    }

    public async Task<PropertyDetailDto?> GetPropertyBySlugAsync(
        string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            var detail = await _httpClient.GetFromJsonAsync<PropertyDetailDto>(
                $"api/properties/slug/{slug}", cancellationToken);
            return NormalizeDetail(detail);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<PropertyTypeDto>> GetPropertyTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<PropertyTypeDto>>(
                "api/propertytypes", cancellationToken) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<Guid?> CreatePropertyAsync(
        CreatePropertyCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/properties", command, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CreatedResourceResponse>(cancellationToken);
                return result?.Id;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("FORBIDDEN");

            return null;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdatePropertyAsync(
        UpdatePropertyCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/properties/{command.Id}", command, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeletePropertyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/properties/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> PublishPropertyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/properties/{id}/publish", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SubmitPropertyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/properties/{id}/submit", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> WithdrawPropertyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/properties/{id}/withdraw", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UploadPropertyImagesAsync(
        Guid propertyId,
        IReadOnlyList<PropertyImageUpload> files,
        CancellationToken cancellationToken = default)
    {
        if (files.Count == 0)
            return true;

        try
        {
            using var content = new MultipartFormDataContent();
            foreach (var file in files)
            {
                var bytes = await ReadUploadBytesAsync(file, cancellationToken);

                var streamContent = new ByteArrayContent(bytes);
                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "files", EnsureImageFileName(file.FileName, file.ContentType));
            }

            var response = await _httpClient.PostAsync(
                $"api/properties/{propertyId}/images/upload", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return false;

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            return !string.IsNullOrWhiteSpace(body) && body != "[]";
        }
        catch
        {
            return false;
        }
    }

    private static async Task<byte[]> ReadUploadBytesAsync(
        PropertyImageUpload file, CancellationToken cancellationToken)
    {
        await using var readStream = file.Content;
        using var buffer = new MemoryStream();
        await readStream.CopyToAsync(buffer, cancellationToken);
        return buffer.ToArray();
    }

    private static string EnsureImageFileName(string fileName, string contentType)
    {
        if (!string.IsNullOrWhiteSpace(Path.GetExtension(fileName)))
            return fileName;

        var ext = contentType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg"
        };

        return string.IsNullOrWhiteSpace(fileName) ? $"image{ext}" : $"{fileName}{ext}";
    }

    private PaginatedList<PropertyDto> NormalizeList(PaginatedList<PropertyDto> result)
    {
        foreach (var item in result.Items)
        {
            if (string.IsNullOrWhiteSpace(item.PrimaryImageUrl))
            {
                item.PrimaryImageUrl = null;
                continue;
            }

            if (!item.PrimaryImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                item.PrimaryImageUrl = new Uri(_httpClient.BaseAddress!, item.PrimaryImageUrl.TrimStart('/')).ToString();
            }
        }

        return result;
    }

    private PropertyDetailDto? NormalizeDetail(PropertyDetailDto? detail)
    {
        if (detail is null)
            return null;

        foreach (var image in detail.Images)
            image.Url = ResolveMediaUrl(image.Url);

        if (!string.IsNullOrWhiteSpace(detail.AgentAvatarUrl))
            detail.AgentAvatarUrl = ResolveMediaUrl(detail.AgentAvatarUrl);

        return detail;
    }

    private string ResolveMediaUrl(string url) => _mediaUrls.Resolve(url) ?? url;

    public async Task<PosterProfileDto?> GetPosterProfileAsync(
        string userId, int pageNumber = 1, int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await _httpClient.GetFromJsonAsync<PosterProfileDto>(
                $"api/posters/{userId}?pageNumber={pageNumber}&pageSize={pageSize}",
                cancellationToken);

            if (profile?.Listings is not null)
                profile.Listings = NormalizeList(profile.Listings);

            if (profile is not null && !string.IsNullOrWhiteSpace(profile.AvatarUrl))
                profile.AvatarUrl = _mediaUrls.Resolve(profile.AvatarUrl);

            return profile;
        }
        catch
        {
            return null;
        }
    }

    private sealed class CreatedResourceResponse
    {
        public Guid Id { get; set; }
    }
}
