using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class UserRegisterViewModel
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(50, MinimumLength = 3)]
    public string HoTen { get; set; }

    [Required(ErrorMessage = "Ngày sinh không được để trống")]
    public DateTime NgaySinh { get; set; }

    [Required(ErrorMessage = "CCCD Không được để trống")]
    [StringLength(12, MinimumLength = 12,ErrorMessage ="Căn cước công dân phải có 12 số")]
    public string CCCD { get; set; }

     [Required(ErrorMessage = "địa chỉ Không được để trống")]
    public string DiaChi { get; set; }

    [Required(ErrorMessage = "số điện thoại Không được để trống")]
    [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    public string SDT { get; set; }

   [Required(ErrorMessage = "Tên đăng nhập Không được để trống")]
    [StringLength(30, MinimumLength = 5)]
    public string TenDanhNhap { get; set; }

     [Required(ErrorMessage = "Mật khẩu Không được để trống")]
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_]).{6,}$", ErrorMessage = "Mật khẩu phải có chữ, số, ký tự đặc biệt và tối thiểu 6 ký tự")]
    public string MatKhau { get; set; }
}