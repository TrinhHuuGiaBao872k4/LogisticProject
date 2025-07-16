using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class LoginViewModel
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [JsonPropertyName("TenDanhNhap")] // ⭐ KHẮC PHỤC LỖI GỬI TÊN KHÔNG TRÙNG
    public string TenDangNhap { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [JsonPropertyName("MatKhau")]
    public string MatKhau { get; set; }
}
public class LoginResultViewModel
{
    public string TenDanhNhap { get; set; }
    public string AccessToken { get; set; }
}