using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LogisticService.Models;
using Microsoft.IdentityModel.Tokens;

public static class JwtTokenGenerator
{
    public static string GenerateToken(NguoiDung user, IConfiguration config)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name,user.TenDanhNhap),
            new Claim(ClaimTypes.Role,user.MaVaiTro?.Trim())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}