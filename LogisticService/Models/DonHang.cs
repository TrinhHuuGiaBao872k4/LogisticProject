using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class DonHang
{
    public string MaDonHang { get; set; } = null!;

    public string? TenDonHang { get; set; }

    public DateTime? NgayKhoiTao { get; set; }

    public DateTime? NgayVanChuyen { get; set; }

    public DateTime? NgayDenDuKien { get; set; }

    public string? MaNguoiDung { get; set; }

    public int? TienShip { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<LichSuTrangThaiDonHang> LichSuTrangThaiDonHangs { get; set; } = new List<LichSuTrangThaiDonHang>();

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }

    public virtual ICollection<PhieuVanChuyen> PhieuVanChuyens { get; set; } = new List<PhieuVanChuyen>();

    public virtual ICollection<TinhTrangDonHangChiTiet> TinhTrangDonHangChiTiets { get; set; } = new List<TinhTrangDonHangChiTiet>();
}
