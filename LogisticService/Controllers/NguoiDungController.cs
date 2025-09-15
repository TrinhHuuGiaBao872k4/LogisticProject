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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterViewModel dto)
    {
        return await _nguoiDungService.RegisterAsync(dto);
    }

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




    // [HttpPost("DangNhap")]
    // public async Task<ActionResult> DangNhap(UserLoginViewModel userLogin)
    // {
    //     var res = await _nguoiDungService.Login(userLogin) as OkObjectResult;
    //     var userResult = res?.Value as HTTPResponseClient<UserLoginResultVM>;
    //     //Tạo cookie từ server 
    //     // var cookieOption =  new CookieOptions(){
    //     //     HttpOnly = true,
    //     //     Secure = true,
    //     //     Expires = DateTime.Now.AddDays(1)
    //     // };
    //     // HttpContext.Response.Cookies.Append("accessToken",userResult.Data.AccessToken,cookieOption );
    //     // Console.WriteLine(@$"token :{ userResult.Data.AccessToken}");
    //     return res;
    // }
    [HttpPost("DangNhap")]
    public async Task<ActionResult<HTTPResponseClient<UserLoginResultVM>>> DangNhap([FromBody] UserLoginViewModel userLogin)
    {
        var response = await _nguoiDungService.Login(userLogin);

        // // Nếu đăng nhập thành công thì tạo cookie
        // if (response.StatusCode == 200 && response.Data?.AccessToken != null)
        // {
        //     var cookieOption = new CookieOptions
        //     {
        //         HttpOnly = true,
        //         Secure = true, // bật khi dùng HTTPS
        //         Expires = DateTime.Now.AddDays(1)
        //     };
        //     HttpContext.Response.Cookies.Append("accessToken", response.Data.AccessToken, cookieOption);
        // }

        // Trả về API response chuẩn
        return StatusCode(response.StatusCode, response);
    }

}

