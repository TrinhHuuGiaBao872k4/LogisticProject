using LogisticService.Helpers;
using LogisticService.Models;

public interface IDonHangService : IServiceBase<DonHang>
{

    Task<string> DatHangAsync(DatHangViewModel model, string userId);
    Task<bool> CapNhatTrangThaiAsync(string maDonHang, UpdateTrangThaiRequest req, string userId, string vaiTro);
    Task<OrderTrackingViewModel?> GetOrderTrackingAsync(string maDonHang);

}
public class DonHangService : ServiceBase<DonHang>, IDonHangService
{
    private readonly IUnitOfWork _unitOfWork;

    public DonHangService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> DatHangAsync(DatHangViewModel model, string userId)
    {
        var maDonHang = IdHelper.GenerateId("DH", 20);
        var trangThai = "TT02"; // Mặc định: Đang xử lý
        bool canGiaoNgay = true;

        try
        {
            await _unitOfWork.BeginTransaction();

            var donHang = new DonHang
            {
                MaDonHang = maDonHang,
                TenDonHang = "Đơn hàng " + maDonHang,
                NgayKhoiTao = DateTime.Now,
                NgayVanChuyen = DateTime.Now.AddDays(1),
                NgayDenDuKien = DateTime.Now.AddDays(2),
                MaNguoiDung = userId,
                TienShip = model.TienShip
            };
            await _unitOfWork.DonHangRepository.AddAsync(donHang);

            foreach (var item in model.DanhSachSanPham)
            {
                var hangHoa = await _unitOfWork.GetRepository<HangHoa>()
                    .SingleOrDefaultAsync(h => h.MaHangHoa == item.MaHangHoa);

                if (hangHoa == null)
                    throw new Exception($"Không tìm thấy hàng hóa {item.MaHangHoa}");

                if (hangHoa.GiaHangHoa == null)
                    throw new Exception($"Hàng hóa {item.MaHangHoa} chưa có giá bán.");

                // ✅ Kiểm tra tồn kho
                var tonKho = await _unitOfWork.DonHangRepository.GetTonKhoAsync(item.MaHangHoa);
                if (item.SoLuong > tonKho)
                    throw new Exception($"Số lượng đặt vượt quá tồn kho của hàng hóa {item.MaHangHoa}");

                // ✅ Nếu đặt bằng đúng tồn kho ⇒ cần xác nhận
                if (item.SoLuong == tonKho)
                    canGiaoNgay = false;

                var chiTiet = new ChiTietDonHang
                {
                    MaChiTietDonHang = IdHelper.GenerateId("CTDH", 20),
                    MaDonHang = maDonHang,
                    MaHangHoa = item.MaHangHoa,
                    SoLuong = item.SoLuong,
                    DonGia = (int)hangHoa.GiaHangHoa.Value
                };
                await _unitOfWork.GetRepository<ChiTietDonHang>().AddAsync(chiTiet);

                // ✅ Trừ tồn kho
                await _unitOfWork.DonHangRepository.TruTonKhoAsync(item.MaHangHoa, item.SoLuong);
            }

            // ✅ Ghi trạng thái
            trangThai = canGiaoNgay ? "TT02" : "TT01"; // TT01 nếu cần xác nhận, TT02 là xử lý luôn

            await _unitOfWork.GetRepository<LichSuTrangThaiDonHang>().AddAsync(new LichSuTrangThaiDonHang
            {
                MaLichSu = IdHelper.GenerateId("LS", 20),
                MaDonHang = maDonHang,
                MaTrangThai = trangThai,
                NgayCapNhat = DateTime.Now,
                GhiChu = canGiaoNgay ? "Đơn hàng đang xử lý ngay" : "Chờ xác nhận do tồn kho vừa hết"
            });

            await _unitOfWork.GetRepository<TinhTrangDonHangChiTiet>().AddAsync(new TinhTrangDonHangChiTiet
            {
                MaTinhTrangChiTiet = IdHelper.GenerateId("TTCT", 20),
                MaDonHang = maDonHang,
                NoiDung = canGiaoNgay ? "Đủ hàng, xử lý ngay" : "Cần xác nhận vì hết tồn",
                ThoiGian = DateTime.Now,
                GhiChu = "Đơn hàng mới"
            });

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();

            return maDonHang;
        }
        catch
        {
            await _unitOfWork.RollBack();
            throw;
        }
    }
    public async Task<bool> CapNhatTrangThaiAsync(string maDonHang, UpdateTrangThaiRequest req, string userId, string vaiTro)
    {
        var donHang = await _unitOfWork.DonHangRepository.GetByIdAsync(maDonHang);
        if (donHang == null) return false;

        var maLichSu = IdHelper.GenerateId("LS", 20);
        await _unitOfWork.GetRepository<LichSuTrangThaiDonHang>().AddAsync(new LichSuTrangThaiDonHang
        {
            MaLichSu = maLichSu,
            MaDonHang = maDonHang,
            MaTrangThai = req.MaTrangThai,
            NgayCapNhat = req.ThoiGian ?? DateTime.Now,
            GhiChu = req.GhiChu ?? $"{vaiTro} ({userId}) cập nhật trạng thái"
        });

        await _unitOfWork.GetRepository<TinhTrangDonHangChiTiet>().AddAsync(new TinhTrangDonHangChiTiet
        {
            MaTinhTrangChiTiet = IdHelper.GenerateId("TTCT", 20),
            MaDonHang = maDonHang,
            NoiDung = req.NoiDungChiTiet ?? $"{vaiTro} cập nhật {req.MaTrangThai}",
            ThoiGian = req.ThoiGian ?? DateTime.Now,
            GhiChu = req.GhiChu
        });

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
    public async Task<OrderTrackingViewModel?> GetOrderTrackingAsync(string maDonHang)
    {
        var donHang = await _unitOfWork.DonHangRepository.GetByIdAsync(maDonHang);
        if (donHang == null) return null;

        // ✅ Lấy lịch sử đúng đơn hàng từ DB, không load tất cả
        var lichSu = await _unitOfWork
            .GetRepository<LichSuTrangThaiDonHang>()
            .WhereAsync(l => l.MaDonHang == maDonHang);

        var tinhTrang = await _unitOfWork
            .GetRepository<TinhTrangDonHangChiTiet>()
            .WhereAsync(t => t.MaDonHang == maDonHang);

        // ✅ Trạng thái hiện tại = bản ghi có NgayCapNhat mới nhất
        var trangThaiHienTai = lichSu
            .OrderBy(l => l.NgayCapNhat)
            .FirstOrDefault()?.MaTrangThai?.Trim();

        return new OrderTrackingViewModel
        {
            MaDonHang = donHang.MaDonHang,
            TenDonHang = donHang.TenDonHang,
            NgayKhoiTao = donHang.NgayKhoiTao,
            TrangThaiHienTai = trangThaiHienTai,
            LichSuTrangThai = lichSu.OrderByDescending(l => l.NgayCapNhat).Select(l => new OrderStatusVM
            {
                MaTrangThai = l.MaTrangThai.Trim(),
                NgayCapNhat = l.NgayCapNhat,
                GhiChu = l.GhiChu
            }).ToList(),
            ChiTietTinhTrang = tinhTrang.OrderByDescending(t => t.ThoiGian).Select(t => new OrderDetailStatusVM
            {
                NoiDung = t.NoiDung,
                ThoiGian = t.ThoiGian,
                GhiChu = t.GhiChu
            }).ToList()
        };
    }


}