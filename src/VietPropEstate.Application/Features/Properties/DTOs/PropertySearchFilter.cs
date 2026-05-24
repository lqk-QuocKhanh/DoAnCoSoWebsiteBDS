using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Properties.DTOs;

/// <summary>Encapsulates all search/filter/sort parameters for property queries.</summary>
public sealed record PropertySearchFilter
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 12;

    // Text
    public string? SearchTerm { get; init; }

    // Classification
    public Guid? PropertyTypeId { get; init; }
    public Guid? TransactionTypeId { get; init; }
    public ListingType? ListingType { get; init; }
    public PropertyStatus? Status { get; init; }

    // Vietnam address
    public int? ProvinceCode { get; init; }
    public int? WardCode { get; init; }
    public string? ProvinceName { get; init; }

    // Ranges
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public decimal? MinArea { get; init; }
    public decimal? MaxArea { get; init; }

    // Specs
    public int? MinBedrooms { get; init; }
    public int? MaxBedrooms { get; init; }
    public int? Bathrooms { get; init; }
    public PropertyDirection? Direction { get; init; }

    // Flags
    public bool? IsFeatured { get; init; }

    // Sorting
    public string SortBy { get; init; } = "CreatedAt";
    public string SortOrder { get; init; } = "Desc";
}
