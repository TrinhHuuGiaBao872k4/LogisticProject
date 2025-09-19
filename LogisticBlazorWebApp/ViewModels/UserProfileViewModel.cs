using System;
using System.ComponentModel.DataAnnotations;
public class UserProfileViewModel
{
    public string HoTen { get; set; }
    public DateTime NgaySinh { get; set; }
    public string Cccd { get; set; }
    public string DiaChi { get; set; }
    public string Sdt { get; set; }
    public string MaVaiTro { get; set; }
}
public class UserProfileUpdateModel
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Họ tên từ 3-50 ký tự")]
    public string HoTen { get; set; }

    [Required(ErrorMessage = "Ngày sinh không được để trống")]
    public DateTime NgaySinh { get; set; }

    [Required(ErrorMessage = "CCCD Không được để trống")]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "Căn cước công dân phải có 12 số")]
    public string Cccd { get; set; }

    [Required(ErrorMessage = "địa chỉ Không được để trống")]
    public string DiaChi { get; set; }

    [Required(ErrorMessage = "số điện thoại Không được để trống")]
    [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    public string Sdt { get; set; }
}