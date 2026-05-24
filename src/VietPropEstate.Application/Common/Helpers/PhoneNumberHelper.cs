using System.Text.RegularExpressions;

namespace VietPropEstate.Application.Common.Helpers;

public static partial class PhoneNumberHelper
{
    private static readonly Regex VnMobileRegex = VnMobilePattern();

    public static string? Normalize(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        if (digits.StartsWith("84", StringComparison.Ordinal) && digits.Length >= 11)
            digits = "0" + digits[2..];

        return digits.Length == 10 && digits.StartsWith('0') ? digits : null;
    }

    public static bool IsValidVietnameseMobile(string? phoneNumber)
        => Normalize(phoneNumber) is not null && VnMobileRegex.IsMatch(Normalize(phoneNumber)!);

    [GeneratedRegex(@"^0(3|5|7|8|9)[0-9]{8}$")]
    private static partial Regex VnMobilePattern();
}
