using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LogisticService.Models;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly LogisticDbServiceContext _context;
    private readonly IConfiguration _config;

    public UserController(LogisticDbServiceContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // 👉 Đăng ký
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var exists = await _context.NguoiDungs.AnyAsync(x => x.TenDanhNhap == model.TenDanhNhap);
        if (exists) return Conflict("Tên đăng nhập đã tồn tại!");

        var hashed = PasswordHasher.HashPassword(model.MatKhau);

        var newUser = new NguoiDung
        {
            MaNguoiDung = $"ND{DateTime.UtcNow.Ticks}",
            HoTen = model.HoTen,
            NgaySinh = model.NgaySinh,
            Cccd = model.CCCD,
            DiaChi = model.DiaChi,
            Sdt = model.SDT,
            TenDanhNhap = model.TenDanhNhap,
            MatKhau = hashed,
            MaVaiTro = "VT003"
        };

        _context.NguoiDungs.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok("Đăng ký thành công!");
    }

    // 👉 Đăng nhập
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginViewModel model)
    {
        var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.TenDanhNhap == model.TenDanhNhap);
        if (user == null || !PasswordHasher.VerifyPassword(model.MatKhau, user.MatKhau))
            return Unauthorized("Sai tên đăng nhập hoặc mật khẩu!");

        var token = JwtTokenGenerator.GenerateToken(user, _config);
        return Ok(new UserResponseViewModel
        {
            HoTen = user.HoTen,
            TenDanhNhap = user.TenDanhNhap,
            Token = token
        });
    }

    // 👉 Lấy thông tin user (yêu cầu JWT)
    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            message = "Lấy thông tin profile thành công",
            tenDangNhap = username,
            vaiTro = role
        });
    }
}