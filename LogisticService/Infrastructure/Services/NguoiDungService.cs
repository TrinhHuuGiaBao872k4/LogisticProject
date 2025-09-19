using LogisticService.Models;
using LogisticService.PasswordHelper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
public interface INguoiDungService : IServiceBase<NguoiDung>
{
    Task<HTTPResponseClient<UserLoginResultVM>> Login(UserLoginViewModel userLogin);
    Task<string> GenerateMaNguoiDungAsync();
    Task<ActionResult> RegisterAsync(UserRegisterViewModel dto);
    Task<ActionResult> UpdateProfileAsync(string userId, UpdateUserViewModel dto);
    Task<ActionResult> ChangePasswordAsync(string userId, ChangePasswordViewModel dto);
}

public class NguoiDungService : ServiceBase<NguoiDung>, INguoiDungService
{
    private readonly JwtAuthService _JwtAuthService;
    public NguoiDungService(IUnitOfWork unitOfWork, JwtAuthService jwtAuthService) : base(unitOfWork)
    {
        _JwtAuthService = jwtAuthService;
    }
    public async Task<HTTPResponseClient<UserLoginResultVM>> Login(UserLoginViewModel userLogin)
    {
        var response = new HTTPResponseClient<UserLoginResultVM>
        {
            DateTime = System.DateTime.Now
        };

        try
        {
            // 1. Kiểm tra input
            if (string.IsNullOrWhiteSpace(userLogin.TenDanhNhap) || string.IsNullOrWhiteSpace(userLogin.MatKhau))
            {
                response.StatusCode = 400;
                response.Message = "Tên đăng nhập và mật khẩu không được để trống";
                return response;
            }

            // 2. Tìm user trong DB
            NguoiDung? userDB = await _repository.SingleOrDefaultAsync(n => n.TenDanhNhap == userLogin.TenDanhNhap);
            if (userDB == null)
            {
                response.StatusCode = 404;
                response.Message = "Người dùng không tồn tại";
                return response;
            }

            // 3. Kiểm tra mật khẩu
            if (!PasswordHelper.VerifyPassword(userLogin.MatKhau, userDB.MatKhau))
            {
                response.StatusCode = 401;
                response.Message = "Sai mật khẩu";
                return response;
            }

            // 4. Tạo token
            string token = _JwtAuthService.GenerateToken(userDB);
            if (string.IsNullOrEmpty(token))
            {
                response.StatusCode = 500;
                response.Message = "Không tạo được token đăng nhập";
                return response;
            }

            // 5. Thành công
            response.StatusCode = 200;
            response.Message = "Đăng nhập thành công";
            response.Data = new UserLoginResultVM
            {
                TenDanhNhap = userLogin.TenDanhNhap,
                AccessToken = token
            };

            return response;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = $"Lỗi hệ thống: {ex.Message}";
            return response;
        }
    }
    public async Task<string> GenerateMaNguoiDungAsync()
    {
        string prefix = "ND";
        string newMa;
        bool exists;

        do
        {
            newMa = $"{prefix}{DateTime.Now.Ticks}";
            exists = await _repository.AnyAsync(n => n.MaNguoiDung == newMa);

            // Nếu muốn an toàn hơn, bạn có thể thêm Task.Delay(1) để tránh trùng Ticks
            if (exists) await Task.Delay(1);

        } while (exists);

        return newMa;
    }
    public async Task<ActionResult> RegisterAsync(UserRegisterViewModel dto)
    {
        // Kiểm tra trùng tên đăng nhập
        var existingUser = await _repository.SingleOrDefaultAsync(u => u.TenDanhNhap == dto.TenDanhNhap);
        if (existingUser != null)
        {
            var conflictResponse = new HTTPResponseClient<object>
            {
                StatusCode = 409,
                Message = "Tên đăng nhập đã tồn tại!",
                DateTime = DateTime.Now,
                Data = null
            };
            return new ConflictObjectResult(conflictResponse);
        }

        // Tạo mã người dùng duy nhất
        string maNguoiDung = await GenerateMaNguoiDungAsync();

        // Hash mật khẩu
        var hashedPassword = PasswordHelper.HashPassword(dto.MatKhau);

        // Tạo đối tượng người dùng mới
        var newUser = new NguoiDung
        {
            MaNguoiDung = maNguoiDung,
            HoTen = dto.HoTen,
            NgaySinh = dto.NgaySinh,
            Cccd = dto.CCCD,
            DiaChi = dto.DiaChi,
            Sdt = dto.SDT,
            TenDanhNhap = dto.TenDanhNhap,
            MatKhau = hashedPassword,
            MaVaiTro = "VT003",
            MaTrangThai = "TT01",
        };

        await _repository.AddAsync(newUser);
        await _uow.SaveChangesAsync();

        var successResponse = new HTTPResponseClient<NguoiDung>
        {
            StatusCode = 200,
            Message = "Đăng ký thành công!",
            DateTime = DateTime.Now,
            Data = newUser
        };

        return new OkObjectResult(successResponse);
    }
    public async Task<ActionResult> UpdateProfileAsync(string maNguoiDung, UpdateUserViewModel dto)
    {
        var nguoiDung = await _repository.SingleOrDefaultAsync(m => m.MaNguoiDung == maNguoiDung);
        if (nguoiDung == null)
        {
            var notFoundResponse = new HTTPResponseClient<object>
            {
                StatusCode = 404,
                Message = "Người dùng không tồn tại.",
                DateTime = DateTime.Now,
                Data = null
            };
            return new NotFoundObjectResult(notFoundResponse);
        }

        // Cập nhật các trường cho phép sửa
        nguoiDung.HoTen = dto.HoTen;
        nguoiDung.NgaySinh = dto.NgaySinh;
        nguoiDung.DiaChi = dto.DiaChi;
        nguoiDung.Sdt = dto.Sdt;

        try
        {
            _repository.Update(nguoiDung);
            await _uow.SaveChangesAsync();

            var successResponse = new HTTPResponseClient<NguoiDung>
            {
                StatusCode = 200,
                Message = "Cập nhật thông tin thành công.",
                DateTime = DateTime.Now,
                Data = nguoiDung
            };
            return new OkObjectResult(successResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new HTTPResponseClient<object>
            {
                StatusCode = 500,
                Message = $"Có lỗi xảy ra: {ex.Message}",
                DateTime = DateTime.Now,
                Data = null
            };
            return new ObjectResult(errorResponse) { StatusCode = 500 };
        }
    }
    public async Task<ActionResult> ChangePasswordAsync(string maNguoiDung, ChangePasswordViewModel dto)
    {
        try
        {
            var nguoiDung = await _repository.SingleOrDefaultAsync(m => m.MaNguoiDung == maNguoiDung);
            if (nguoiDung == null)
            {
                return new NotFoundObjectResult(new HTTPResponseClient<object>
                {
                    StatusCode = 404,
                    Message = "Người dùng không tồn tại.",
                    DateTime = DateTime.Now,
                    Data = null
                });
            }

            if (!PasswordHelper.VerifyPassword(dto.MatKhauCu, nguoiDung.MatKhau))
            {
                return new BadRequestObjectResult(new HTTPResponseClient<object>
                {
                    StatusCode = 400,
                    Message = "Mật khẩu cũ không đúng.",
                    DateTime = DateTime.Now,
                    Data = null
                });
            }

            if (dto.MatKhauMoi == dto.MatKhauCu)
            {
                return new BadRequestObjectResult(new HTTPResponseClient<object>
                {
                    StatusCode = 400,
                    Message = "Mật khẩu mới không được trùng mật khẩu cũ.",
                    DateTime = DateTime.Now,
                    Data = null
                });
            }

            nguoiDung.MatKhau = PasswordHelper.HashPassword(dto.MatKhauMoi);
            _repository.Update(nguoiDung);
            await _uow.SaveChangesAsync();

            return new OkObjectResult(new HTTPResponseClient<NguoiDung>
            {
                StatusCode = 200,
                Message = "Đổi mật khẩu thành công.",
                DateTime = DateTime.Now,
                Data = nguoiDung
            });
        }
        catch (Exception ex)
        {
            return new ObjectResult(new HTTPResponseClient<object>
            {
                StatusCode = 500,
                Message = $"Có lỗi xảy ra: {ex.Message}",
                DateTime = DateTime.Now,
                Data = null
            })
            {
                StatusCode = 500
            };
        }
    }

}