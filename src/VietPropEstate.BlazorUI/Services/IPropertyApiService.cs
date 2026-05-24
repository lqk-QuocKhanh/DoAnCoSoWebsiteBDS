using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.Commands.CreateProperty;
using VietPropEstate.Application.Features.Properties.Commands.UpdateProperty;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.Services;

public interface IPropertyApiService
{
    Task<PaginatedList<PropertyDto>?> GetPropertiesAsync(
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
        CancellationToken cancellationToken = default);

    Task<PropertyDetailDto?> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PropertyDetailDto?> GetPropertyBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<List<PropertyTypeDto>> GetPropertyTypesAsync(CancellationToken cancellationToken = default);

    Task<Guid?> CreatePropertyAsync(CreatePropertyCommand command, CancellationToken cancellationToken = default);

    Task<bool> UpdatePropertyAsync(UpdatePropertyCommand command, CancellationToken cancellationToken = default);

    Task<bool> DeletePropertyAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> PublishPropertyAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> SubmitPropertyAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> WithdrawPropertyAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> UploadPropertyImagesAsync(
        Guid propertyId,
        IReadOnlyList<PropertyImageUpload> files,
        CancellationToken cancellationToken = default);

    Task<PosterProfileDto?> GetPosterProfileAsync(
        string userId, int pageNumber = 1, int pageSize = 12,
        CancellationToken cancellationToken = default);
}

public sealed class PropertyImageUpload
{
    public required Stream Content { get; init; }
    public required string FileName { get; init; }
    public string ContentType { get; init; } = "image/jpeg";
}
