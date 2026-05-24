using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using VietPropEstate.IntegrationTests.Support;

namespace VietPropEstate.IntegrationTests;

public sealed class PropertiesApiTests(VietPropEstateWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetPropertyTypes_ReturnsSeededTypes()
    {
        var response = await Client.GetAsync("/api/propertytypes");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var types = await response.Content.ReadFromJsonAsync<List<PropertyTypeResponse>>();
        types.Should().NotBeNull();
        types!.Count.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task GetProperties_ReturnsActiveListings()
    {
        var response = await Client.GetAsync("/api/properties?pageNumber=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PagedPropertiesResponse>();
        payload.Should().NotBeNull();
        payload!.Items.Should().NotBeEmpty();
        payload.Items.Should().Contain(p => p.Title == "Integration Test Listing");
    }

    [Fact]
    public async Task GetPropertyBySlug_ReturnsActiveListing()
    {
        var listResponse = await Client.GetFromJsonAsync<PagedPropertiesResponse>("/api/properties?pageSize=1");
        var slug = listResponse!.Items.First().Slug;

        var response = await Client.GetAsync($"/api/properties/slug/{slug}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetFeaturedProperties_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/properties/featured?count=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private sealed class PropertyTypeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private sealed class PagedPropertiesResponse
    {
        public List<PropertyItemResponse> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }

    private sealed class PropertyItemResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }
}
