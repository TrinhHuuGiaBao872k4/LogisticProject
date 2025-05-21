using LogisticService.Models;
using Microsoft.EntityFrameworkCore;
public interface IDonHangRepository : IRepository<DonHang>
    {
    Task<bool> TonKhoDuAsync(string maHangHoa, int soLuong);
    Task TruTonKhoAsync(string maHangHoa, int soLuong);
    Task<List<string>> LayDanhSachHangHetAsync(List<ChiTietDatHangViewModel> danhSach);
    }
public class DonHangRepository : Repository<DonHang>, IDonHangRepository
{
    private readonly LogisticDbServiceContext _context;

    public DonHangRepository(LogisticDbServiceContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> TonKhoDuAsync(string maHangHoa, int soLuong)
    {
        var ton = await _context.TonKhos.FirstOrDefaultAsync(t => t.MaHangHoa == maHangHoa);
        return ton != null && ton.SoLuongTon >= soLuong;
    }

    public async Task TruTonKhoAsync(string maHangHoa, int soLuong)
    {
        var ton = await _context.TonKhos.FirstOrDefaultAsync(t => t.MaHangHoa == maHangHoa);
        if (ton != null)
        {
            ton.SoLuongTon -= soLuong;
            _context.TonKhos.Update(ton);
        }
    }

    public async Task<List<string>> LayDanhSachHangHetAsync(List<ChiTietDatHangViewModel> danhSach)
    {
        var hetHang = new List<string>();
        foreach (var item in danhSach)
        {
            var du = await TonKhoDuAsync(item.MaHangHoa, item.SoLuong);
            if (!du)
                hetHang.Add(item.MaHangHoa);
        }
        return hetHang;
    }
}