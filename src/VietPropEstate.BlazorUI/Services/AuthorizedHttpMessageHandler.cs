using System.Net.Http.Headers;
using VietPropEstate.BlazorUI.States;

namespace VietPropEstate.BlazorUI.Services;

public sealed class AuthorizedHttpMessageHandler : DelegatingHandler
{
    private readonly AuthState _authState;
    private readonly IAuthTokenStorage _tokenStorage;

    public AuthorizedHttpMessageHandler(AuthState authState, IAuthTokenStorage tokenStorage)
    {
        _authState = authState;
        _tokenStorage = tokenStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _authState.CurrentUser?.AccessToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            var stored = await _tokenStorage.LoadAsync();
            if (stored is not null && !string.IsNullOrWhiteSpace(stored.AccessToken))
            {
                _authState.SetUser(stored);
                token = stored.AccessToken;
            }
        }

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
