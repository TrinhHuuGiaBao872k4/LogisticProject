using LogisticService.Models;
using Microsoft.EntityFrameworkCore;
    public interface IDonHangRepository : IRepository<DonHang>
    {
 
    Task<bool> TonKhoDuAsync(string maHangHoa, int soLuong);
    Task TruTonKhoAsync(string maHangHoa, int soLuong);
       Task<int> GetTonKhoAsync(string maHangHoa); // 
    Task<List<string>> LayDanhSachHangHetAsync(List<ChiTietDatHangViewModel> danhSach);
    }
    
    public class DonHangRepository : Repository<DonHang>, IDonHangRepository
    {
    private readonly LogisticDbServiceContext _context;

    public DonHangRepository(LogisticDbServiceContext context) : base(context)
    {
        _context = context;
    }
    public async Task<int> GetTonKhoAsync(string maHangHoa)
    {
    var tonKhoList = await _context.TonKhos
        .Where(t => t.MaHangHoa == maHangHoa)
        .ToListAsync();
    // Sử dụng GetValueOrDefault() vì SoLuongTon là nullable
    return tonKhoList.Sum(t => t.SoLuongTon.GetValueOrDefault());
    }
    public async Task<bool> TonKhoDuAsync(string maHangHoa, int soLuong)
    {
        var ton = await _context.TonKhos.FirstOrDefaultAsync(t => t.MaHangHoa == maHangHoa);
        return ton != null && ton.SoLuongTon >= soLuong;
    }

    public async Task TruTonKhoAsync(string maHangHoa, int soLuong)
    {
        var ton = await _context.TonKhos.FirstOrDefaultAsync(t => t.MaHangHoa == maHangHoa);
        if (ton == null || ton.SoLuongTon < soLuong)
            throw new Exception($"Không đủ tồn kho để trừ hàng hóa {maHangHoa}");
        if (ton != null)
        {
            ton.SoLuongTon -= soLuong;
            _context.TonKhos.Update(ton);
        }
    }
    public async Task HoanTonKhoAsync(string maHangHoa, int soLuong)
    {
        var ton = await _context.TonKhos.FirstOrDefaultAsync(t => t.MaHangHoa == maHangHoa);
        if (ton != null)
        {
            ton.SoLuongTon += soLuong;
            _context.TonKhos.Update(ton);
            await _context.SaveChangesAsync();
        }
        else
        {
             // Nếu chưa có dòng tồn kho, tạo mới luôn
        var newTon = new TonKho
        {
            MaHangHoa = maHangHoa,
            MaKhoHang = "KH001", // 🟡 Bạn có thể sửa theo logic kho phù hợp
            SoLuongTon = soLuong
        };
        await _context.TonKhos.AddAsync(newTon);
        await _context.SaveChangesAsync();
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