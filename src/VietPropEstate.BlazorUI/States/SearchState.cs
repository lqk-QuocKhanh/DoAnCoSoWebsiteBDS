using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.States;

public sealed class SearchState
{
    public string? SearchTerm { get; set; }
    public ListingType? ListingType { get; set; }
    public int? ProvinceCode { get; set; }
    public string? ProvinceName { get; set; }
    public int? WardCode { get; set; }
    public string? WardName { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinArea { get; set; }
    public decimal? MaxArea { get; set; }
    public int? MinBedrooms { get; set; }
    public Guid? PropertyTypeId { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "Desc";

    public event Action? OnChange;

    public void Reset()
    {
        SearchTerm = null;
        ListingType = null;
        ProvinceCode = null;
        ProvinceName = null;
        WardCode = null;
        WardName = null;
        MinPrice = null;
        MaxPrice = null;
        MinArea = null;
        MaxArea = null;
        MinBedrooms = null;
        PropertyTypeId = null;
        SortBy = "CreatedAt";
        SortOrder = "Desc";
        OnChange?.Invoke();
    }

    public void Apply() => OnChange?.Invoke();
}
