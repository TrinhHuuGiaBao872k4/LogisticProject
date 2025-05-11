using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class TinhTrangDonHangChiTiet
{
    public string MaTinhTrangChiTiet { get; set; } = null!;

    public string? MaDonHang { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? ThoiGian { get; set; }

    public string? GhiChu { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }
}
