public class OrderTrackingViewModel
{
    public string MaDonHang { get; set; }
    public string? TenDonHang { get; set; }
    public DateTime? NgayKhoiTao { get; set; }
    public string? TrangThaiHienTai { get; set; }

    public List<OrderStatusVM> LichSuTrangThai { get; set; } = new();
    public List<OrderDetailStatusVM> ChiTietTinhTrang { get; set; } = new();
}

public class OrderStatusVM
{
    public string? MaTrangThai { get; set; }
    public DateTime? NgayCapNhat { get; set; }
    public string? GhiChu { get; set; }
}

public class OrderDetailStatusVM
{
    public string? NoiDung { get; set; }
    public DateTime? ThoiGian { get; set; }
    public string? GhiChu { get; set; }
}
