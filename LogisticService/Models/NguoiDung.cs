using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class NguoiDung
{
    public string MaNguoiDung { get; set; } = null!;

    public string? HoTen { get; set; }

    public DateTime? NgaySinh { get; set; }

    public string? Cccd { get; set; }

    public string? DiaChi { get; set; }

    public string? Sdt { get; set; }

    public string? MaVaiTro { get; set; }

    public string? TenDanhNhap { get; set; }

    public string? MatKhau { get; set; }

    public string? MaTrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();

    public virtual TrangThaiNguoiDung? MaTrangThaiNavigation { get; set; }

    public virtual VaiTro? MaVaiTroNavigation { get; set; }

    public virtual ICollection<PhieuNhapXuat> PhieuNhapXuats { get; set; } = new List<PhieuNhapXuat>();
}
