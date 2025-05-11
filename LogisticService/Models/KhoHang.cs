using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class KhoHang
{
    public string MaKhoHang { get; set; } = null!;

    public string? MaLoaiKhoHang { get; set; }

    public string? TenKhoHang { get; set; }

    public string? DiaChi { get; set; }

    public int SucChua { get; set; }

    public virtual ICollection<ChiTietPhieuNhapXuat> ChiTietPhieuNhapXuats { get; set; } = new List<ChiTietPhieuNhapXuat>();

    public virtual LoaiKhoHang? MaLoaiKhoHangNavigation { get; set; }

    public virtual ICollection<TonKho> TonKhos { get; set; } = new List<TonKho>();
}
