namespace VietPropEstate.Application.Common.Constants;

public static class CacheKeys
{
    public const string PropertyTypes = "lookup:property-types";
    public const string TransactionTypes = "lookup:transaction-types";
    public const string VipPackages = "lookup:vip-packages";
    public const string Provinces = "lookup:provinces";

    public static string Wards(int provinceCode) => $"lookup:wards:{provinceCode}";
}

public static class CacheDurations
{
    public static readonly TimeSpan LookupData = TimeSpan.FromHours(1);
    public static readonly TimeSpan VipPackages = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan AddressData = TimeSpan.FromHours(24);
}
