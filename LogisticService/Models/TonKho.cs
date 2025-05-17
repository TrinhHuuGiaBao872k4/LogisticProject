using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class TonKho
{
    public string MaKhoHang { get; set; } = null!;

    public string MaHangHoa { get; set; } = null!;

    public int? SoLuongTon { get; set; }

    public virtual HangHoa MaHangHoaNavigation { get; set; } = null!;

    public virtual KhoHang MaKhoHangNavigation { get; set; } = null!;
}
