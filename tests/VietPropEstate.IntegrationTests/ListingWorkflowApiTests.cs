using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using VietPropEstate.IntegrationTests.Support;

namespace VietPropEstate.IntegrationTests;

public sealed class ListingWorkflowApiTests(VietPropEstateWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Customer_CreateSubmit_AdminPublish_FullWorkflow()
    {
        var customerEmail = $"broker-{Guid.NewGuid():N}@test.vietpropestate.vn";
        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email = customerEmail,
            password = "Test@12345",
            confirmPassword = "Test@12345",
            firstName = "Workflow",
            lastName = "Tester",
            role = "Customer"
        });
        registerResponse.EnsureSuccessStatusCode();

        var customerToken = await Client.LoginAndGetTokenAsync(customerEmail, "Test@12345");
        Client.SetBearerToken(customerToken);

        const string phone = "0987654321";
        var sendCodeResponse = await Client.PostAsJsonAsync("/api/auth/phone/send-code", new { phoneNumber = phone });
        sendCodeResponse.EnsureSuccessStatusCode();
        var sendCodeBody = await sendCodeResponse.Content.ReadFromJsonAsync<SendPhoneCodeResponse>();

        var verifyResponse = await Client.PostAsJsonAsync("/api/auth/phone/verify", new
        {
            phoneNumber = phone,
            code = sendCodeBody!.DevCode
        });
        verifyResponse.EnsureSuccessStatusCode();

        var types = await Client.GetFromJsonAsync<List<PropertyTypeResponse>>("/api/propertytypes");
        var typeId = types!.First().Id;

        var createResponse = await Client.PostAsJsonAsync("/api/properties", new
        {
            title = "Workflow Test Listing",
            description = "Created by integration test",
            price = 2_500_000_000m,
            currency = "VND",
            area = 70m,
            listingType = 0,
            numberOfBedrooms = 2,
            numberOfBathrooms = 1,
            street = "99 Workflow Street",
            province = "TP.HCM",
            district = "Quận 1",
            ward = "Phường Bến Nghé",
            propertyTypeId = typeId,
            agentId = Guid.Empty
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<CreatedPropertyResponse>();
        created.Should().NotBeNull();

        var submitResponse = await Client.PostAsync($"/api/properties/{created!.Id}/submit", null);
        submitResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        Client.DefaultRequestHeaders.Authorization = null;
        var adminToken = await Client.LoginAndGetTokenAsync("admin@vietpropestate.vn", "Admin@123");
        Client.SetBearerToken(adminToken);

        var publishResponse = await Client.PostAsync($"/api/properties/{created.Id}/publish", null);
        publishResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        Client.DefaultRequestHeaders.Authorization = null;
        var publicResponse = await Client.GetAsync($"/api/properties/{created.Id}");
        publicResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private sealed class PropertyTypeResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class CreatedPropertyResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class SendPhoneCodeResponse
    {
        public string? DevCode { get; set; }
    }
}
