using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public sealed class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenState _tokenState;

    public JwtAuthStateProvider(TokenState tokenState)
        => _tokenState = tokenState;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        var token = _tokenState.AccessToken;

        if (!string.IsNullOrWhiteSpace(token) && !JwtHelper.IsExpired(token))
        {
            var claims = new List<Claim>();

            var name = JwtHelper.GetClaim(token, "unique_name")
                       ?? JwtHelper.GetClaim(token, "name")
                       ?? JwtHelper.GetClaim(token, "preferred_username");

            if (!string.IsNullOrWhiteSpace(name))
                claims.Add(new Claim(ClaimTypes.Name, name));

            foreach (var role in JwtHelper.GetRoles(token))
                claims.Add(new Claim(ClaimTypes.Role, role));

            identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        }

        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }

    // Gọi sau khi set/clear token để UI & [Authorize] cập nhật ngay
    public void ForceRefresh() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
