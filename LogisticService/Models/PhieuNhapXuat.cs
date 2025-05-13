using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class PhieuNhapXuat
{
    public string MaPhieuNhapXuat { get; set; } = null!;

    public string? LoaiPhieu { get; set; }

    public DateTime? NgayLapPhieu { get; set; }

    public string? GhiChu { get; set; }

    public string? MaNguoiDung { get; set; }

    public virtual ICollection<ChiTietPhieuNhapXuat> ChiTietPhieuNhapXuats { get; set; } = new List<ChiTietPhieuNhapXuat>();

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
