using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisticService.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly LogisticDBServiceContext _context;
    private readonly IConfiguration _config;

    public UserController(LogisticDBServiceContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // Đăng ký
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterViewModel dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _context.NguoiDungs
            .AnyAsync(u => u.TenDanhNhap == dto.TenDanhNhap);

        if (existingUser)
            return Conflict("Tên đăng nhập đã tồn tại!");

        var hashedPassword = PasswordHasher.HashPassword(dto.MatKhau);

        var newUser = new NguoiDung
        {
            MaNguoiDung = $"ND{DateTime.UtcNow.Ticks}",
            HoTen = dto.HoTen,
            NgaySinh = dto.NgaySinh,
            Cccd = dto.CCCD,
            DiaChi = dto.DiaChi,
            Sdt = dto.SDT,
            TenDanhNhap = dto.TenDanhNhap,
            MatKhau = hashedPassword,
            MaVaiTro = "VT003"
        };

        _context.NguoiDungs.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok("Đăng ký thành công!");
    }

    // Đăng nhập
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginViewModel dto)
    {
        var user = await _context.NguoiDungs
            .FirstOrDefaultAsync(u => u.TenDanhNhap == dto.TenDanhNhap);

        if (user == null || !PasswordHasher.VerifyPassword(dto.MatKhau, user.MatKhau))
            return Unauthorized("Sai tên đăng nhập hoặc mật khẩu!");

        var token = JwtTokenGenerator.GenerateToken(user, _config);

        return Ok(new UserResponseViewModel
        {
            HoTen = user.HoTen,
            TenDanhNhap = user.TenDanhNhap,
            Token = token
        });
    }

    // Lấy thông tin profile
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

