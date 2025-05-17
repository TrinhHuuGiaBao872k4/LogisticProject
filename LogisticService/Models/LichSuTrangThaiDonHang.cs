using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class LichSuTrangThaiDonHang
{
    public string MaLichSu { get; set; } = null!;

    public string? MaDonHang { get; set; }

    public string? MaTrangThai { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public string? GhiChu { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }

    public virtual TrangThaiDonHang? MaTrangThaiNavigation { get; set; }
}
