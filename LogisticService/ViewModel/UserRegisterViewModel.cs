using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class UserRegisterViewModel
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(50, MinimumLength = 3)]
    public string HoTen { get; set; }

    [Required]
    public DateTime NgaySinh { get; set; }

    [Required]
    [StringLength(12, MinimumLength = 12)]
    public string CCCD { get; set; }

    [Required]
    public string DiaChi { get; set; }

    [Required]
    [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    public string SDT { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 5)]
    public string TenDanhNhap { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_]).{6,}$", ErrorMessage = "Mật khẩu phải có chữ, số, ký tự đặc biệt và tối thiểu 6 ký tự")]
    public string MatKhau { get; set; }
}