public class OrderStatusNotificationVM
{
    public string MaDonHang { get; set; } = null!;
    public string MaTrangThai { get; set; } = null!;
    public string? NoiDung { get; set; }
    public DateTime ThoiGian { get; set; }
    public string? GhiChu { get; set; }
}

public class UpdateTrangThaiRequest
    {
        public string MaTrangThai { get; set; } = null!;
        public string? GhiChu { get; set; }
        public string? NoiDungChiTiet { get; set; } // nội dung mô tả ngắn
        public DateTime? ThoiGian { get; set; } // optional
    }
