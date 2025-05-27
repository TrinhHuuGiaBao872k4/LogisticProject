using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisticService.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
[Route("api/[controller]")]
[ApiController]
public class UserController(LogisticDbServiceContext _context, IConfiguration _config, JwtAuthService _jwt, INguoiDungService _nguoiDungService) : ControllerBase
{
    // private readonly LogisticDbServiceContext _context;
    // private readonly IConfiguration _config;

    // public UserController(LogisticDbServiceContext context, IConfiguration config)
    // {
    //     _context = context;
    //     _config = config;
    // }

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
        if (user.MaTrangThai.Trim() != "TT01")
            return Unauthorized("Tài khoản của bạn không ở trạng thái hoạt động.");
        //  Chặn đăng nhập nếu là Admin hoặc SuperAdmin
        if (user.MaVaiTro?.Trim().ToUpper() == "VT000" || user.MaVaiTro?.Trim().ToUpper() == "VT001")
        {
            Console.WriteLine(">> Chặn đăng nhập vai trò Admin hoặc SuperAdmin");
            return Forbid("Tài khoản này không được phép đăng nhập vào hệ thống!");
        }

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
            vaiTro = role,
        });
    }
    [Authorize]
    [HttpPut("Update-Profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserViewModel dto)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        if (username == null)
            return Unauthorized("Không xác định được người dùng.");

        var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDanhNhap == username);
        if (user == null)
            return NotFound("Người dùng không tồn tại.");

        // Cập nhật các trường được phép sửa
        user.HoTen = dto.HoTen;
        user.NgaySinh = dto.NgaySinh;
        user.DiaChi = dto.DiaChi;
        user.Sdt = dto.Sdt;
        // Không được cập nhật CanCuoc (Căn cước)

        _context.NguoiDungs.Update(user);
        await _context.SaveChangesAsync();

        return Ok("Cập nhật thông tin thành công.");
    }
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel dto)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        if (username == null)
            return Unauthorized("Không xác định được người dùng.");

        var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDanhNhap == username);
        if (user == null)
            return NotFound("Người dùng không tồn tại.");

        // Kiểm tra mật khẩu cũ
        if (!PasswordHasher.VerifyPassword(dto.MatKhauCu, user.MatKhau))
            return BadRequest(" Mật khẩu cũ không đúng.");

        // Hash lại mật khẩu mới
        user.MatKhau = PasswordHasher.HashPassword(dto.MatKhauMoi);

        _context.NguoiDungs.Update(user);
        await _context.SaveChangesAsync();

        return Ok(" Đổi mật khẩu thành công.");
    }


    

    [HttpPost("/NguoiDung/DangNhap")]
    public async Task<ActionResult> DangNhap(UserLoginViewModel userLogin)
    {
        var res = await _nguoiDungService.Login(userLogin) as OkObjectResult;
        var userResult = res?.Value as HTTPResponseClient<UserLoginResultVM>;
        //Tạo cookie từ server 
        // var cookieOption =  new CookieOptions(){
        //     HttpOnly = true,
        //     Secure = true,
        //     Expires = DateTime.Now.AddDays(1)
        // };
        // HttpContext.Response.Cookies.Append("accessToken",userResult.Data.AccessToken,cookieOption );
        // Console.WriteLine(@$"token :{ userResult.Data.AccessToken}");
        return res;
    }

}

