using System;
using System.Collections.Generic;


public partial class HangHoaVM
{
    public string MaHangHoa { get; set; } = null!;

    public string? MaLoaiHangHoa { get; set; }

    public string? TenHangHoa { get; set; }

    public DateTime? NgaySanXuat { get; set; }

    public string? HinhAnh { get; set; }

    public decimal? GiaHangHoa { get; set; }

    public string MaNguoiDung { get; set; } = null!;

    public virtual ICollection<ChiTietDonHangVM> ChiTietDonHangs { get; set; } = new List<ChiTietDonHangVM>();

    public virtual ICollection<ChiTietPhieuNhapXuatVM> ChiTietPhieuNhapXuats { get; set; } = new List<ChiTietPhieuNhapXuatVM>();

    public virtual ICollection<DonHangCungCapVM> DonHangCungCaps { get; set; } = new List<DonHangCungCapVM>();

    public virtual LoaiHangHoa? MaLoaiHangHoaNavigation { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual ICollection<TonKhoVM> TonKhos { get; set; } = new List<TonKho>();
}
public partial class ChiTietDonHangVM
{
    public string MaHangHoa { get; set; } = null!;

    public string MaDonHang { get; set; } = null!;

    public int? DonGia { get; set; }

    public int? SoLuong { get; set; }

    public string MaChiTietDonHang { get; set; } = null!;

    public virtual DonHangVM MaDonHangNavigation { get; set; } = null!;

    public virtual HangHoaVM MaHangHoaNavigation { get; set; } = null!;
}
public partial class ChiTietPhieuNhapXuatVM
{
    public string MaPhieuNhapXuat { get; set; } = null!;

    public string MaHangHoa { get; set; } = null!;

    public string MaKhoHang { get; set; } = null!;

    public int? SoLuong { get; set; }

    public virtual HangHoaVM MaHangHoaNavigation { get; set; } = null!;

    public virtual KhoHangVM MaKhoHangNavigation { get; set; } = null!;

    public virtual PhieuNhapXuat MaPhieuNhapXuatNavigation { get; set; } = null!;
}
public partial class DonHangCungCapVM
{
    public string MaHangHoa { get; set; } = null!;

    public string MaNhaCungCap { get; set; } = null!;

    public DateTime? NgayCungCap { get; set; }

    public virtual HangHoaVM MaHangHoaNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNhaCungCapNavigation { get; set; } = null!;
}

public partial class TonKhoVM
{
    public string MaKhoHang { get; set; } = null!;

    public string MaHangHoa { get; set; } = null!;

    public int? SoLuongTon { get; set; }

    public virtual HangHoaVM MaHangHoaNavigation { get; set; } = null!;

    public virtual KhoHangVM MaKhoHangNavigation { get; set; } = null!;
}
public partial class DonHangVM
{
    public string MaDonHang { get; set; } = null!;

    public string? TenDonHang { get; set; }

    public DateTime? NgayKhoiTao { get; set; }

    public DateTime? NgayVanChuyen { get; set; }

    public DateTime? NgayDenDuKien { get; set; }

    public string? MaNguoiDung { get; set; }

    public int? TienShip { get; set; }

    public virtual ICollection<ChiTietDonHangVM> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<LichSuTrangThaiDonHang> LichSuTrangThaiDonHangs { get; set; } = new List<LichSuTrangThaiDonHang>();

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }

    public virtual ICollection<PhieuVanChuyen> PhieuVanChuyens { get; set; } = new List<PhieuVanChuyen>();

    public virtual ICollection<TinhTrangDonHangChiTiet> TinhTrangDonHangChiTiets { get; set; } = new List<TinhTrangDonHangChiTiet>();
}


public partial class KhoHangVM
{
    public string MaKhoHang { get; set; } = null!;

    public string? MaLoaiKhoHang { get; set; }

    public string? TenKhoHang { get; set; }

    public string? DiaChi { get; set; }

    public int SucChua { get; set; }

    public virtual ICollection<ChiTietPhieuNhapXuat> ChiTietPhieuNhapXuats { get; set; } = new List<ChiTietPhieuNhapXuat>();

    public virtual LoaiKhoHang? MaLoaiKhoHangNavigation { get; set; }

    public virtual ICollection<TonKhoVM> TonKhos { get; set; } = new List<TonKhoVM>();
}
