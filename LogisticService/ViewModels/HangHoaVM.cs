using System;
using System.Collections.Generic;

namespace LogisticService.ViewModels;

public partial class HangHoaVM
{
    public string MaHangHoa { get; set; } = null!;

    public string? MaLoaiHangHoa { get; set; }

    public string? TenHangHoa { get; set; }

    public DateTime? NgaySanXuat { get; set; }

    public string? HinhAnh { get; set; }
    public decimal GiaHangHoa { get; set; }
}
