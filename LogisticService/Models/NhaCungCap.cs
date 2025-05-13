using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class NhaCungCap
{
    public string MaNhaCungCap { get; set; } = null!;

    public string? TenNhaCungCap { get; set; }

    public virtual ICollection<DonHangCungCap> DonHangCungCaps { get; set; } = new List<DonHangCungCap>();
}
