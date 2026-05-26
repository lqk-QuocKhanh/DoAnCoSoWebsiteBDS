using VietPropEstate.BlazorUI.Services;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.Helpers;

public static class DisplayLabels
{
    private static readonly Dictionary<string, (string Vi, string En)> PropertyTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Apartment"] = ("Căn hộ", "Apartment"),
            ["House"] = ("Nhà phố", "House"),
            ["Villa"] = ("Biệt thự", "Villa"),
            ["Land"] = ("Đất nền", "Land"),
            ["Office"] = ("Văn phòng", "Office"),
            ["Motel"] = ("Phòng trọ", "Motel"),
            ["Căn hộ"] = ("Căn hộ", "Apartment"),
            ["Nhà phố"] = ("Nhà phố", "House"),
            ["Biệt thự"] = ("Biệt thự", "Villa"),
            ["Đất nền"] = ("Đất nền", "Land"),
            ["Văn phòng"] = ("Văn phòng", "Office"),
            ["Phòng trọ"] = ("Phòng trọ", "Motel")
        };

    private static readonly Dictionary<string, (string Vi, string En)> TransactionTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Sell"] = ("Bán", "For Sale"),
            ["Rent"] = ("Cho thuê", "For Rent"),
            ["Bán"] = ("Bán", "For Sale"),
            ["Cho thuê"] = ("Cho thuê", "For Rent")
        };

    public static string PropertyType(string? name, AppLanguage language)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        if (PropertyTypes.TryGetValue(name.Trim(), out var labels))
            return language == AppLanguage.Vietnamese ? labels.Vi : labels.En;

        return name;
    }

    public static string TransactionType(string? name, AppLanguage language)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        if (TransactionTypes.TryGetValue(name.Trim(), out var labels))
            return language == AppLanguage.Vietnamese ? labels.Vi : labels.En;

        return name;
    }

    public static string GetListingType(ListingType type, AppLanguage language) => (type, language) switch
    {
        (ListingType.ForSale, AppLanguage.Vietnamese) => "Bán",
        (ListingType.ForSale, AppLanguage.English) => "For Sale",
        (ListingType.ForRent, AppLanguage.Vietnamese) => "Cho thuê",
        (ListingType.ForRent, AppLanguage.English) => "For Rent",
        (ListingType.ForLease, AppLanguage.Vietnamese) => "Thuê dài hạn",
        (ListingType.ForLease, AppLanguage.English) => "For Lease",
        _ => type.ToString()
    };

    public static string Direction(PropertyDirection direction, AppLanguage language) => (direction, language) switch
    {
        (PropertyDirection.North, AppLanguage.Vietnamese) => "Bắc",
        (PropertyDirection.North, AppLanguage.English) => "North",
        (PropertyDirection.South, AppLanguage.Vietnamese) => "Nam",
        (PropertyDirection.South, AppLanguage.English) => "South",
        (PropertyDirection.East, AppLanguage.Vietnamese) => "Đông",
        (PropertyDirection.East, AppLanguage.English) => "East",
        (PropertyDirection.West, AppLanguage.Vietnamese) => "Tây",
        (PropertyDirection.West, AppLanguage.English) => "West",
        (PropertyDirection.NorthEast, AppLanguage.Vietnamese) => "Đông Bắc",
        (PropertyDirection.NorthEast, AppLanguage.English) => "Northeast",
        (PropertyDirection.NorthWest, AppLanguage.Vietnamese) => "Tây Bắc",
        (PropertyDirection.NorthWest, AppLanguage.English) => "Northwest",
        (PropertyDirection.SouthEast, AppLanguage.Vietnamese) => "Đông Nam",
        (PropertyDirection.SouthEast, AppLanguage.English) => "Southeast",
        (PropertyDirection.SouthWest, AppLanguage.Vietnamese) => "Tây Nam",
        (PropertyDirection.SouthWest, AppLanguage.English) => "Southwest",
        _ => direction.ToString()
    };
}
