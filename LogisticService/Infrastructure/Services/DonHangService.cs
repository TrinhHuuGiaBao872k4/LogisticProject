using LogisticService.Models;

public interface IDonHangService
{

    Task<string> DatHangAsync(DatHangViewModel model);
    
}
public class DonHangService : IDonHangService
{
    private readonly IUnitOfWork _unitOfWork;

    public DonHangService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> DatHangAsync(DatHangViewModel model)
    {
        var maDonHang = "DH" + DateTime.Now.Ticks;
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
                MaNguoiDung = model.MaNguoiDung,
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
                MaLichSu = "LS" + DateTime.Now.Ticks,
                MaDonHang = maDonHang,
                MaTrangThai = trangThai,
                NgayCapNhat = DateTime.Now,
                GhiChu = canGiaoNgay ? "Đơn hàng đang xử lý ngay" : "Chờ xác nhận do tồn kho vừa hết"
            });

            await _unitOfWork.GetRepository<TinhTrangDonHangChiTiet>().AddAsync(new TinhTrangDonHangChiTiet
            {
                MaTinhTrangChiTiet = "TTCT" + DateTime.Now.Ticks,
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
}