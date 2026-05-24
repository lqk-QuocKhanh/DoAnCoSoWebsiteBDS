using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using VietPropEstate.IntegrationTests.Support;

namespace VietPropEstate.IntegrationTests;

public sealed class FavoritesApiTests(VietPropEstateWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ToggleFavorite_AddAndRemove_Works()
    {
        var token = await Client.LoginAndGetTokenAsync("admin@vietpropestate.vn", "Admin@123");
        Client.SetBearerToken(token);

        var listings = await Client.GetFromJsonAsync<PagedPropertiesResponse>("/api/properties?pageSize=1");
        var propertyId = listings!.Items.First().Id;

        var addResponse = await Client.PostAsJsonAsync($"/api/favorites/{propertyId}", new { note = "Test favorite" });
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var addPayload = await addResponse.Content.ReadFromJsonAsync<ToggleFavoriteResponse>();
        addPayload!.Added.Should().BeTrue();

        var statusResponse = await Client.GetFromJsonAsync<FavoriteStatusResponse>($"/api/favorites/{propertyId}/status");
        statusResponse!.IsFavorited.Should().BeTrue();

        var removeResponse = await Client.PostAsJsonAsync($"/api/favorites/{propertyId}", new { note = (string?)null });
        removeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var removePayload = await removeResponse.Content.ReadFromJsonAsync<ToggleFavoriteResponse>();
        removePayload!.Added.Should().BeFalse();
    }

    private sealed class PagedPropertiesResponse
    {
        public List<PropertyItemResponse> Items { get; set; } = [];
    }

    private sealed class PropertyItemResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class ToggleFavoriteResponse
    {
        public bool Added { get; set; }
    }

    private sealed class FavoriteStatusResponse
    {
        public bool IsFavorited { get; set; }
    }
}
