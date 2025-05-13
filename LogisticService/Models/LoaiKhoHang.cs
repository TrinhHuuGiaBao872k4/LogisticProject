using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class LoaiKhoHang
{
    public string MaLoaiKhoHang { get; set; } = null!;

    public string? TenLoaiKhoHang { get; set; }

    public virtual ICollection<KhoHang> KhoHangs { get; set; } = new List<KhoHang>();
}
