using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Ban đầu trả về anonymous, sẽ cập nhật sau khi client lấy token
        return Task.FromResult(new AuthenticationState(_anonymous));
    }

    public void MarkUserAsAuthenticated(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, jwtToken.Claims.First(c => c.Type == "TenDanhNhap").Value)
        };

        claims.AddRange(jwtToken.Claims
        .Where(c => c.Type == "role")
        .Select(r => new Claim(ClaimTypes.Role, r.Value)));

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void MarkUserAsLoggedOut()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
