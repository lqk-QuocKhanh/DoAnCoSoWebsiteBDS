using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using VietPropEstate.IntegrationTests.Support;

namespace VietPropEstate.IntegrationTests;

public sealed class PhoneVerificationListingTests(VietPropEstateWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Customer_CannotCreateListing_WithoutPhoneVerification()
    {
        var email = $"customer-{Guid.NewGuid():N}@test.vietpropestate.vn";
        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = "Test@12345",
            confirmPassword = "Test@12345",
            firstName = "Phone",
            lastName = "Gate",
            role = "Customer"
        });
        registerResponse.EnsureSuccessStatusCode();

        var token = await Client.LoginAndGetTokenAsync(email, "Test@12345");
        Client.SetBearerToken(token);

        var types = await Client.GetFromJsonAsync<List<PropertyTypeResponse>>("/api/propertytypes");
        var typeId = types!.First().Id;

        var createResponse = await Client.PostAsJsonAsync("/api/properties", new
        {
            title = "Should Fail Without Phone",
            description = "Blocked by phone verification",
            price = 1_000_000_000m,
            currency = "VND",
            area = 60m,
            listingType = 0,
            street = "1 Test Street",
            province = "TP.HCM",
            district = "Quận 1",
            ward = "Phường 1",
            propertyTypeId = typeId,
            agentId = Guid.Empty
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Customer_VerifyPhone_ThenCreateListing_Succeeds()
    {
        var email = $"customer-{Guid.NewGuid():N}@test.vietpropestate.vn";
        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = "Test@12345",
            confirmPassword = "Test@12345",
            firstName = "Verified",
            lastName = "Customer",
            role = "Customer"
        });
        registerResponse.EnsureSuccessStatusCode();

        var token = await Client.LoginAndGetTokenAsync(email, "Test@12345");
        Client.SetBearerToken(token);

        const string phone = "0912345678";
        var sendResponse = await Client.PostAsJsonAsync("/api/auth/phone/send-code", new { phoneNumber = phone });
        sendResponse.EnsureSuccessStatusCode();

        var sendBody = await sendResponse.Content.ReadFromJsonAsync<SendPhoneCodeResponse>();
        sendBody.Should().NotBeNull();
        sendBody!.DevCode.Should().NotBeNullOrWhiteSpace();

        var verifyResponse = await Client.PostAsJsonAsync("/api/auth/phone/verify", new
        {
            phoneNumber = phone,
            code = sendBody.DevCode
        });
        verifyResponse.EnsureSuccessStatusCode();

        var types = await Client.GetFromJsonAsync<List<PropertyTypeResponse>>("/api/propertytypes");
        var typeId = types!.First().Id;

        var createResponse = await Client.PostAsJsonAsync("/api/properties", new
        {
            title = "Verified Customer Listing",
            description = "Created after phone verification",
            price = 2_000_000_000m,
            currency = "VND",
            area = 70m,
            listingType = 0,
            street = "2 Test Street",
            province = "TP.HCM",
            district = "Quận 1",
            ward = "Phường 2",
            propertyTypeId = typeId,
            agentId = Guid.Empty
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private sealed class PropertyTypeResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class SendPhoneCodeResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? DevCode { get; set; }
    }
}
