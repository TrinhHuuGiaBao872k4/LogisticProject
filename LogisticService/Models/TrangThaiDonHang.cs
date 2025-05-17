using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class TrangThaiDonHang
{
    public string MaTrangThai { get; set; } = null!;

    public string? TenTrangThai { get; set; }

    public virtual ICollection<LichSuTrangThaiDonHang> LichSuTrangThaiDonHangs { get; set; } = new List<LichSuTrangThaiDonHang>();
}
