public class DonHangViewModel
{
    public string MaDonHang { get; set; } = null!;
    public DateTime? NgayDat { get; set; }
    public DateTime? NgayVanChuyen { get; set; }
    public DateTime? NgayDenDuKien { get; set; }
    public int? TienShip { get; set; }
    public string TrangThai { get; set; } = null!;

    // Thêm mới
    public string? Customer { get; set; }
    public decimal TongTien { get; set; }



    public string NgayDatDisplay => NgayDat?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
    public string NgayVanChuyenDisplay => NgayVanChuyen?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa vận chuyển";
    public string NgayDenDuKienDisplay => NgayDenDuKien?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có";
}
