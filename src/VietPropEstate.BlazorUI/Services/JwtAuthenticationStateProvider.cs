using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using VietPropEstate.BlazorUI.Models;
using VietPropEstate.BlazorUI.States;

namespace VietPropEstate.BlazorUI.Services;

public sealed class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthState _authState;

    public JwtAuthenticationStateProvider(AuthState authState) => _authState = authState;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(BuildState(_authState.CurrentUser));

    public void NotifyUserChanged(AuthResponse? user)
        => NotifyAuthenticationStateChanged(Task.FromResult(BuildState(user)));

    private static AuthenticationState BuildState(AuthResponse? user)
    {
        if (user is null || string.IsNullOrWhiteSpace(user.AccessToken))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new("email_verified", user.EmailConfirmed ? "true" : "false")
        };

        foreach (var role in user.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
