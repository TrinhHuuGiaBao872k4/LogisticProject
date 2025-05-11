using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class PhieuVanChuyen
{
    public string MaPhieuVanChuyen { get; set; } = null!;

    public string? MaDonVi { get; set; }

    public string? MaDonHang { get; set; }

    public DateTime? NgayVanChuyen { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }

    public virtual DonViVanChuyen? MaDonViNavigation { get; set; }
}
