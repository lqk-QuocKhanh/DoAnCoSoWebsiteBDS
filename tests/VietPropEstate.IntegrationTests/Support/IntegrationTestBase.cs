namespace VietPropEstate.IntegrationTests.Support;

public abstract class IntegrationTestBase : IClassFixture<VietPropEstateWebApplicationFactory>, IAsyncLifetime
{
    protected IntegrationTestBase(VietPropEstateWebApplicationFactory factory)
    {
        Factory = factory;
    }

    protected VietPropEstateWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await Factory.EnsureSeededAsync();
        Client = Factory.CreateClient();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
