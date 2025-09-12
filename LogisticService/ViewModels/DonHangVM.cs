using System;
using System.Collections.Generic;

namespace LogisticService.ViewModels
{
    public class DonHangUserVM
    {
        public string MaDonHang { get; set; } = null!;
        public DateTime? NgayDat { get; set; }
        public DateTime? NgayVanChuyen { get; set; }
        public DateTime? NgayDenDuKien { get; set; }
        public int? TienShip { get; set; }

        public string TrangThai { get; set; } = null!;

    }
}
