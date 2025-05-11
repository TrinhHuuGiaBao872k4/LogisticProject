using System;
using System.Collections.Generic;

namespace LogisticService.Models;

public partial class LoaiHangHoa
{
    public string MaLoaiHangHoa { get; set; } = null!;

    public string? TenLoaiHangHoa { get; set; }

    public virtual ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
}
