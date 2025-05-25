using LogisticService.Models;
using LogisticService.PasswordHelper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
public interface INguoiDungService : IServiceBase<NguoiDung>
{
    Task<ActionResult> Login(UserLoginViewModel userLogin);
}

public class NguoiDungService : ServiceBase<NguoiDung>, INguoiDungService
{
    private readonly JwtAuthService _JwtAuthService;
    public NguoiDungService(IUnitOfWork unitOfWork, JwtAuthService jwtAuthService) : base(unitOfWork)
    {
        _JwtAuthService = jwtAuthService;
    }
    public async Task<ActionResult> Login(UserLoginViewModel userLogin)
    {
        //Kiểm tra user trong database
        NguoiDung? userDB = await _repository.SingleOrDefaultAsync(n => n.TenDanhNhap == userLogin.TenDanhNhap);
        if (userDB != null && PasswordHelper.VerifyPassword(userLogin.MatKhau, userDB.MatKhau))
        {
            // Đăng nhập thành công
            //Tạo token trả vào userLoginResult
            UserLoginResultVM usResult = new UserLoginResultVM();
            usResult.TenDanhNhap = userLogin.TenDanhNhap;
            usResult.AccessToken = _JwtAuthService.GenerateToken(userDB);
            var resOb = new HTTPResponseClient<UserLoginResultVM>()
            {
                StatusCode = 200,
                Message = "Successfully",
                DateTime = DateTime.Now,
                Data = usResult
            };
            
            return new OkObjectResult(resOb);
        }
        var failOb = new HTTPResponseClient<UserLoginResultVM>()
        {
            StatusCode = 400,
            Message = "Login fail",
            DateTime = DateTime.Now,
            Data = null
        };
        return new BadRequestObjectResult(failOb);
    }
    // public Task<dynamic> Register(dynamic newUser)
    // {
    //     throw new NotImplementedException();
    // }
}