using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class DonViVanChuyen
{
    public string MaDonVi { get; set; } = null!;

    public string? TenDonVi { get; set; }

    public virtual ICollection<PhieuVanChuyen> PhieuVanChuyens { get; set; } = new List<PhieuVanChuyen>();
}
