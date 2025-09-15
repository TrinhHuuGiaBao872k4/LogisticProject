using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class HangHoaVM
{
    public string MaHangHoa { get; set; } = null!;

    public string? MaLoaiHangHoa { get; set; }

    public string? TenHangHoa { get; set; }
    public string? HinhAnh { get; set; }

    public DateTime? NgaySanXuat { get; set; }

    public decimal? GiaHangHoa { get; set; }

    public string MaNguoiDung { get; set; } = null!;
    
}
public class HangHoaCreateVM
{
    [Required(ErrorMessage = "Mã loại hàng hóa là bắt buộc")]
    public string MaLoaiHangHoa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên hàng hóa là bắt buộc")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Tên hàng hóa từ 3-150 ký tự")]
    public string TenHangHoa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày sản xuất là bắt buộc")]

    public DateTime NgaySanXuat { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Hình ảnh là bắt buộc")]
    [Url(ErrorMessage = "Hình ảnh phải là URL hợp lệ")]
    public string HinhAnh { get; set; } = string.Empty;

    [Range(0, 100000000000, ErrorMessage = "Giá phải >= 0")]
    public decimal GiaHangHoa { get; set; }
}
