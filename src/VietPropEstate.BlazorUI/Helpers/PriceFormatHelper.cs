using System.Globalization;

namespace VietPropEstate.BlazorUI.Helpers;

public static class PriceFormatHelper
{
    public static string FormatVnd(decimal amount) =>
        amount.ToString("#,##0", CultureInfo.InvariantCulture).Replace(",", ".") + "đ";
}
