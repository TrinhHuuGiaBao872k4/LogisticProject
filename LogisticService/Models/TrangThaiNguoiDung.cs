using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class TrangThaiNguoiDung
{
    public string MaTrangThai { get; set; } = null!;

    public string? TenTrangThai { get; set; }

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
