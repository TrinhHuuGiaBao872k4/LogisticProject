using System;
using System.Collections.Generic;

namespace LogisticService.ViewModels;

public partial class HangHoaVM
{

    public string? MaLoaiHangHoa { get; set; }

    public string? TenHangHoa { get; set; }

    public DateTime? NgaySanXuat { get; set; }

    public string? HinhAnh { get; set; }
    public decimal? GiaHangHoa { get; set; }

}
public class HangHoaReturnResult
{
    public string MaHangHoa { get; set; }
    public string MaLoaiHangHoa { get; set; }
    public string TenHangHoa { get; set; }
    public DateTime? NgaySanXuat { get; set; }
    public string HinhAnh { get; set; }
    public decimal? GiaHangHoa { get; set; }
    public string MaNguoiDung { get; set; }
    public int SoLuongTonKho { get; set; }
}