using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class ChiTietDonHang
{
    public string MaHangHoa { get; set; } = null!;

    public string MaDonHang { get; set; } = null!;

    public int? DonGia { get; set; }

    public int? SoLuong { get; set; }

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;

    public virtual HangHoa MaHangHoaNavigation { get; set; } = null!;
}
