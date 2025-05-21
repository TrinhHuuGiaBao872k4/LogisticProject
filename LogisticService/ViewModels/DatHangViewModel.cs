public class DatHangViewModel
{
    public string MaNguoiDung { get; set; }
    public List<ChiTietDatHangViewModel> DanhSachSanPham { get; set; }
    public int TienShip { get; set; }
}

public class ChiTietDatHangViewModel
{
    public string MaHangHoa { get; set; }
    public int SoLuong { get; set; }
    public int DonGia { get; set; }
}