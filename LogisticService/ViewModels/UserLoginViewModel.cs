using System.ComponentModel.DataAnnotations;

public class UserLoginViewModel
{
   [Required(ErrorMessage = "Tên đăng nhập Không được để trống")]
    public string TenDanhNhap { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    public string MatKhau { get; set; }
}