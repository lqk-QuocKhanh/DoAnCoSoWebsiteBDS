using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using VietPropEstate.IntegrationTests.Support;

namespace VietPropEstate.IntegrationTests;

public sealed class VipPackagesApiTests(VietPropEstateWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetVipPackages_ReturnsSeededPackages()
    {
        var response = await Client.GetAsync("/api/vip-packages");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var packages = await response.Content.ReadFromJsonAsync<List<VipPackageResponse>>();
        packages.Should().NotBeNull();
        packages!.Count.Should().BeGreaterThanOrEqualTo(4);
        packages.Should().Contain(p => p.Name == "Cơ Bản");
    }

    private sealed class VipPackageResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
