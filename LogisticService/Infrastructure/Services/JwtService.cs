using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LogisticService.Models;
using Microsoft.EntityFrameworkCore;

public class JwtAuthService
{
    private readonly string? _key;
    private readonly string? _issuer;
    private readonly string? _audience;
    private readonly LogisticDbServiceContext _context;
    public JwtAuthService(IConfiguration Configuration, LogisticDbServiceContext db)
    {
        _key = Configuration["jwt:Secret-Key"];
        _issuer = Configuration["jwt:Issuer"];
        _audience = Configuration["jwt:Audience"];
        _context = db;
    }

    public string GenerateToken(NguoiDung userLogin, int minutes=60)
    {
        // Khóa bí mật để ký token
        var key = Encoding.ASCII.GetBytes(_key);
        // Tạo danh sách các claims cho token
        var claims = new List<Claim>
        {
            new Claim("MaNguoiDung", userLogin.MaNguoiDung),
            new Claim("TenDanhNhap", userLogin.TenDanhNhap),               // Claim mặc định cho username
            // new Claim(ClaimTypes.Role, userLogin.Role),                   // Claim mặc định cho Role
            new Claim(JwtRegisteredClaimNames.Sub, userLogin.TenDanhNhap),   // Subject của token
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique ID của token
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()) // Thời gian tạo token
        };
        List<string> lstRole = new List<string>();
        var userRoles = _context.Database.SqlQueryRaw<string>($@"SELECT VT.TenVaiTro FROM NguoiDung ND INNER JOIN VaiTro VT ON ND.MaVaiTro = VT.MaVaiTro WHERE ND.MaNguoiDung = '{userLogin.MaNguoiDung};'").ToList();
        foreach (var item in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, item));
        }
        // foreach (var role in userRoles)
        // {
        //     claims.Add(new Claim(ClaimTypes.Role, role));
        // }
        // "role": ["admin", "user"], // Nếu nhiều vai trò, có thể là mảng

        // Tạo khóa bí mật để ký token
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        );


        // Thiết lập thông tin cho token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(minutes), // Token hết hạn sau 1 giờ
            SigningCredentials = credentials,
            Issuer = _issuer,                 // Thêm Issuer vào token
            Audience = _audience,              // Thêm Audience vào token

        };
        // Tạo token bằng JwtSecurityTokenHandler
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        // Trả về chuỗi token đã mã hóa
        return tokenHandler.WriteToken(token);
    }

    public string DecodePayloadToken(string token)
    {
        try
        {
            // Kiểm tra token có null hoặc rỗng không
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token không được để trống", nameof(token));
            }
            // Tạo handler và đọc token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            // Lấy username từ claims (thường nằm trong claim "sub" hoặc "name")
            var useridClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "MaNguoiDung"); // Common in some identity providers
            if (useridClaim == null)
            {
                throw new InvalidOperationException("Không tìm thấy userid trong payload");
            }
            return useridClaim.Value;
        }
        catch (Exception ex)
        {
            // Xử lý lỗi (có thể log lỗi ở đây)
            throw new InvalidOperationException($"Lỗi khi decode token: {ex.Message}", ex);
        }
    }
    public TokenResult DecodePayloadTokenInfo(string token)
    {
        try
        {
            // Kiểm tra token có null hoặc rỗng không
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token không được để trống", nameof(token));
            }
            // Tạo handler và đọc token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            // Lấy username từ claims (thường nằm trong claim "sub" hoặc "name")
            var usernameClaim = jwtToken.Claims.FirstOrDefault(); // Common in some identity providers
            if (usernameClaim == null)
            {
                throw new InvalidOperationException("Không tìm thấy username trong payload");
            }
            var tokenResult = new TokenResult
            {
                UserName = jwtToken.Claims.FirstOrDefault(c => c.Type == "TenDanhNhap")?.Value,
                Sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value,
                Jti = jwtToken.Claims.FirstOrDefault(c => c.Type == "jti")?.Value,
                Iat = int.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == "iat")?.Value, out var iat) ? iat : 0,
                Nbf = int.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == "nbf")?.Value, out var nbf) ? nbf : 0,
                Exp = int.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value, out var exp) ? exp : 0,
                Iss = jwtToken.Issuer,
                Aud = jwtToken.Audiences.FirstOrDefault()
            };

            return tokenResult;
        }
        catch (Exception ex)
        {
            // Xử lý lỗi (có thể log lỗi ở đây)
          return null;
        }
    }

    

    // public string? RefreshToken(string expiredToken)
    // {
    //     try
    //     {
    //         var tokenHandler = new JwtSecurityTokenHandler();
    //         var principal = DecodePayloadTokenInfo(expiredToken);

    //         if (principal == null)
    //         {
    //             return null;
    //         }

    //         // Chuyển từ Unix timestamp sang DateTime
    //         var expirationTime = DateTimeOffset.FromUnixTimeSeconds(principal.Exp).UtcDateTime;
    //         var rft = _context.RefreshTokens.FirstOrDefault(token => token.Token == expiredToken);
    //         // So sánh với thời gian hiện tại (UTC)
    //         if (DateTime.UtcNow > expirationTime || DateTime.Now > rft.ExpiresAt)
    //         {
    //             //Hết hạn thì bắt đăng nhập lại
    //             return null;
    //         }
    //         User? user = _context.Users.FirstOrDefault(u => u.Email == principal.UserName | u.Username == principal.UserName);
    //         // Tạo token mới
    //         return GenerateToken(user);
    //     }
    //     catch (Exception ex)
    //     {
    //         return null;
    //     }
    // }



}

