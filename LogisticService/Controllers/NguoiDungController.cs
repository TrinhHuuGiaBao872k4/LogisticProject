using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisticService.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.OutputCaching;
[Route("api/[controller]")]
[ApiController]
public class NguoiDungController(LogisticDbServiceContext _context, IConfiguration _config, JwtAuthService _jwt, INguoiDungService _nguoiDungService) : ControllerBase
{
    // private readonly LogisticDbServiceContext _context;
    // private readonly IConfiguration _config;

    // public UserController(LogisticDbServiceContext context, IConfiguration config)
    // {
    //     _context = context;
    //     _config = config;
    // }

    // Đăng ký
    // [HttpPost("register")]
    // public async Task<IActionResult> Register(UserRegisterViewModel dto)
    // {
    //     if (!ModelState.IsValid)
    //         return BadRequest(ModelState);

    //     var existingUser = await _context.NguoiDungs
    //         .AnyAsync(u => u.TenDanhNhap == dto.TenDanhNhap);

    //     if (existingUser)
    //         return Conflict("Tên đăng nhập đã tồn tại!");

    //     var hashedPassword = PasswordHasher.HashPassword(dto.MatKhau);

    //     var newUser = new NguoiDung
    //     {
    //         MaNguoiDung = $"ND{DateTime.UtcNow.Ticks}",
    //         HoTen = dto.HoTen,
    //         NgaySinh = dto.NgaySinh,
    //         Cccd = dto.CCCD,
    //         DiaChi = dto.DiaChi,
    //         Sdt = dto.SDT,
    //         TenDanhNhap = dto.TenDanhNhap,
    //         MatKhau = hashedPassword,
    //         MaVaiTro = "VT003"
    //     };

    //     _context.NguoiDungs.Add(newUser);
    //     await _context.SaveChangesAsync();

    //     return Ok("Đăng ký thành công!");
    // }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterViewModel dto)
    {
        return await _nguoiDungService.RegisterAsync(dto);
    }

    // Đăng nhập
    // [HttpPost("login")]
    // public async Task<IActionResult> Login(UserLoginViewModel dto)
    // {
    //     var user = await _context.NguoiDungs
    //         .FirstOrDefaultAsync(u => u.TenDanhNhap == dto.TenDanhNhap);

    //     if (user == null || !PasswordHasher.VerifyPassword(dto.MatKhau, user.MatKhau))
    //         return Unauthorized("Sai tên đăng nhập hoặc mật khẩu!");
    //     if (user.MaTrangThai.Trim() != "TT01")
    //         return Unauthorized("Tài khoản của bạn không ở trạng thái hoạt động.");
    //     //  Chặn đăng nhập nếu là Admin hoặc SuperAdmin
    //     if (user.MaVaiTro?.Trim().ToUpper() == "VT000" || user.MaVaiTro?.Trim().ToUpper() == "VT001")
    //     {
    //         Console.WriteLine(">> Chặn đăng nhập vai trò Admin hoặc SuperAdmin");
    //         return Forbid("Tài khoản này không được phép đăng nhập vào hệ thống!");
    //     }

    //     var token = JwtTokenGenerator.GenerateToken(user, _config);

    //     return Ok(new UserResponseViewModel
    //     {
    //         HoTen = user.HoTen,
    //         TenDanhNhap = user.TenDanhNhap,
    //         Token = token
    //     });
    // }

    // Lấy thông tin profile
    [Authorize]
    [OutputCache(Duration = 60, VaryByHeaderNames = new[] { "Authorization" })]
    [HttpGet("profile")]
    public async Task<ActionResult> GetProfile([FromHeader] string authorization)
    {
        var header = HttpContext.Request.Headers;
        var token = header["Authorization"].First().Substring(7);
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new HTTPResponseClient<object>
            {
                StatusCode = 401,
                Data = null,
                DateTime = DateTime.Now,
                Message = "Token không hợp lệ"
            });
        }
        TokenResult res = _jwt.DecodePayloadTokenInfo(token);

        // var username = User.FindFirstValue(ClaimTypes.Name);
        // var role = User.FindFirstValue(ClaimTypes.Role);
        NguoiDung nguoiDung = await _nguoiDungService.GetByIdAsync(res.Id);
        if (nguoiDung == null)
        {
            return BadRequest(new HTTPResponseClient<NguoiDung>
            {
                StatusCode = 400,
                Data = null,
                DateTime = DateTime.Now,
                Message = "Không lấy được thông tin người dùng"
            });
        }
        else
        {
            return Ok(new HTTPResponseClient<NguoiDung>
                {
                    StatusCode = 200,
                    Data = nguoiDung,
                    DateTime = DateTime.Now,
                    Message = "Successfully"
                });
        }
    }
    [Authorize]
    [OutputCache(Duration = 60, VaryByHeaderNames = new[] { "Authorization" })]
    [HttpPut("Update-Profile")]
    public async Task<ActionResult> UpdateProfile([FromBody] UpdateUserViewModel dto)
    {
        var header = HttpContext.Request.Headers;
        var token = header["Authorization"].First().Substring(7);
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new HTTPResponseClient<object>
            {
                StatusCode = 401,
                Data = null,
                DateTime = DateTime.Now,
                Message = "Token không hợp lệ"
            });
        }
        TokenResult res = _jwt.DecodePayloadTokenInfo(token);
        return await _nguoiDungService.UpdateProfileAsync(res.Id, dto);
    }
    [Authorize]
    [OutputCache(Duration = 60, VaryByHeaderNames = new[] { "Authorization" })]
    [HttpPut("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordViewModel dto)
    {
        var header = HttpContext.Request.Headers;
        var token = header["Authorization"].First().Substring(7);
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new HTTPResponseClient<object>
            {
                StatusCode = 401,
                Data = null,
                DateTime = DateTime.Now,
                Message = "Token không hợp lệ"
            });
        }
        TokenResult res = _jwt.DecodePayloadTokenInfo(token);
        return await _nguoiDungService.ChangePasswordAsync(res.Id, dto);
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

