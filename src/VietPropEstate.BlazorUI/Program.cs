using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using VietPropEstate.BlazorUI;
using VietPropEstate.BlazorUI.Configuration;
using VietPropEstate.BlazorUI.Services;
using VietPropEstate.BlazorUI.States;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUri = ApiUrlResolver.Resolve(builder.Configuration);

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<IAuthTokenStorage, AuthTokenStorage>();
builder.Services.AddScoped<AuthorizedHttpMessageHandler>();

builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthorizedHttpMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler) { BaseAddress = apiUri };
});

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 300;
    config.SnackbarConfiguration.SnackbarVariant = MudBlazor.Variant.Filled;
});

builder.Services.AddScoped<IMediaUrlResolver, MediaUrlResolver>();
builder.Services.AddScoped<IPropertyApiService, PropertyApiService>();
builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
builder.Services.AddScoped<IAddressApiClient, AddressApiClient>();
builder.Services.AddScoped<IPaymentApiClient, PaymentApiClient>();
builder.Services.AddScoped<IDashboardApiClient, DashboardApiClient>();
builder.Services.AddScoped<IFavoritesApiClient, FavoritesApiClient>();
builder.Services.AddScoped<INotificationsApiClient, NotificationsApiClient>();
builder.Services.AddScoped<IChatApiClient, ChatApiClient>();
builder.Services.AddScoped<IAdminApiClient, AdminApiClient>();

builder.Services.AddScoped<SearchState>();
builder.Services.AddScoped<NotificationState>();
builder.Services.AddScoped<ChatState>();
builder.Services.AddScoped<AdminState>();

var host = builder.Build();

var authClient = host.Services.GetRequiredService<IAuthApiClient>();
await authClient.InitializeAsync();

await host.RunAsync();
