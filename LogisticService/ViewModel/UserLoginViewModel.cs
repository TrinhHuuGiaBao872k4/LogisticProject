using System.ComponentModel.DataAnnotations;

public class UserLoginViewModel
{
    [Required]
    public string TenDanhNhap { get; set; }

    [Required]
    public string MatKhau { get; set; }
}