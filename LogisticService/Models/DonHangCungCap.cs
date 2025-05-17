using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class DonHangCungCap
{
    public string MaHangHoa { get; set; } = null!;

    public string MaNhaCungCap { get; set; } = null!;

    public DateTime? NgayCungCap { get; set; }

    public virtual HangHoa MaHangHoaNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNhaCungCapNavigation { get; set; } = null!;
}
