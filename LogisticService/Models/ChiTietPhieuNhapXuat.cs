using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class ChiTietPhieuNhapXuat
{
    public string MaPhieuNhapXuat { get; set; } = null!;

    public string MaHangHoa { get; set; } = null!;

    public string MaKhoHang { get; set; } = null!;

    public int? SoLuong { get; set; }

    public virtual HangHoa MaHangHoaNavigation { get; set; } = null!;

    public virtual KhoHang MaKhoHangNavigation { get; set; } = null!;

    public virtual PhieuNhapXuat MaPhieuNhapXuatNavigation { get; set; } = null!;
}
