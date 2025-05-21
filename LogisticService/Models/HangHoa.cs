using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class HangHoa
{
    public string MaHangHoa { get; set; } = null!;

    public string? MaLoaiHangHoa { get; set; }

    public string? TenHangHoa { get; set; }

    public DateTime? NgaySanXuat { get; set; }

    public string? HinhAnh { get; set; }

    public decimal? GiaHangHoa { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietPhieuNhapXuat> ChiTietPhieuNhapXuats { get; set; } = new List<ChiTietPhieuNhapXuat>();

    public virtual ICollection<DonHangCungCap> DonHangCungCaps { get; set; } = new List<DonHangCungCap>();

    public virtual LoaiHangHoa? MaLoaiHangHoaNavigation { get; set; }

    public virtual ICollection<TonKho> TonKhos { get; set; } = new List<TonKho>();
}
