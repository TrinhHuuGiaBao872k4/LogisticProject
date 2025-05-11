using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class VaiTro
{
    public string MaVaiTro { get; set; } = null!;

    public string? TenVaiTro { get; set; }

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
