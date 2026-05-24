using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using VietPropEstate.IntegrationTests.Support;

namespace VietPropEstate.IntegrationTests;

public sealed class AuthApiTests(VietPropEstateWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Login_AdminCredentials_ReturnsToken()
    {
        var token = await Client.LoginAndGetTokenAsync("admin@vietpropestate.vn", "Admin@123");

        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@vietpropestate.vn",
            password = "WrongPassword!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_NewCustomer_ReturnsSuccess()
    {
        var email = $"customer-{Guid.NewGuid():N}@test.vietpropestate.vn";
        var response = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = "Test@12345",
            confirmPassword = "Test@12345",
            firstName = "Test",
            lastName = "Customer",
            role = "Customer"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
