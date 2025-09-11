using System;

namespace LogisticService.ViewModels
{
    public class ChiTietDonHangVM
    {
        public string MaChiTietDonHang { get; set; } = null!;
        public string MaHangHoa { get; set; } = null!;
        public string TenHangHoa { get; set; } = null!;
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public int ThanhTien => SoLuong * DonGia; // Tính tổng tiền tự động
    }
}
