using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using API.APIService;

namespace BlazorCocktails.Client.Services;

public class JwtAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private ClaimsPrincipal _current = new(new ClaimsIdentity());
    private static readonly JwtSecurityTokenHandler _jwtHandler = new();

    public JwtAuthStateProvider()
    {
        _current = BuildPrincipal(APIClient.Token);
        APIClient.TokenChanged += OnTokenChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_current));

    private void OnTokenChanged()
    {
        _current = BuildPrincipal(APIClient.Token);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_current)));
    }

    public void Dispose() => APIClient.TokenChanged -= OnTokenChanged;

    private static ClaimsPrincipal BuildPrincipal(string? jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt))
            return Anonymous();

        try
        {
            var token = _jwtHandler.ReadJwtToken(jwt);

            // Si el token está expirado, tratamos como anónimo
            var exp = token.Claims.FirstOrDefault(c => c.Type is "exp")?.Value;
            if (exp is not null && IsExpired(exp))
                return Anonymous();

            // Creamos identidad con TODAS las claims del token
            var identity = new ClaimsIdentity(token.Claims, authenticationType: "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return Anonymous();
        }

        static ClaimsPrincipal Anonymous() => new(new ClaimsIdentity());
    }

    private static bool IsExpired(string expClaim)
    {
        // "exp" es segundos UNIX
        if (!long.TryParse(expClaim, out var seconds)) return false;
        var expUtc = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        return expUtc <= DateTime.UtcNow;
    }
}
