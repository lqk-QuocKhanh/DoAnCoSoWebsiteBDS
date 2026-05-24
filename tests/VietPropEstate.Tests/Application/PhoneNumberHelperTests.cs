using FluentAssertions;
using VietPropEstate.Application.Common.Helpers;

namespace VietPropEstate.Tests.Application;

public sealed class PhoneNumberHelperTests
{
    [Theory]
    [InlineData("0912345678", "0912345678")]
    [InlineData("+84912345678", "0912345678")]
    [InlineData("84912345678", "0912345678")]
    [InlineData("0912 345 678", "0912345678")]
    public void Normalize_ValidNumbers_ReturnsTenDigitFormat(string input, string expected)
        => PhoneNumberHelper.Normalize(input).Should().Be(expected);

    [Theory]
    [InlineData("123")]
    [InlineData("")]
    [InlineData(null)]
    public void Normalize_InvalidNumbers_ReturnsNull(string? input)
        => PhoneNumberHelper.Normalize(input).Should().BeNull();

    [Fact]
    public void IsValidVietnameseMobile_AcceptsCommonPrefixes()
    {
        PhoneNumberHelper.IsValidVietnameseMobile("0912345678").Should().BeTrue();
        PhoneNumberHelper.IsValidVietnameseMobile("0327890430").Should().BeTrue();
        PhoneNumberHelper.IsValidVietnameseMobile("0212345678").Should().BeFalse();
    }
}
