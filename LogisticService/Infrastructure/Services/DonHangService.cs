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

        var hetHang = await _unitOfWork.DonHangRepository
            .LayDanhSachHangHetAsync(model.DanhSachSanPham);

        var maTrangThai = hetHang.Count > 0 ? "TT02" : "TT01"; // TT01: đủ, TT02: thiếu

        try
        {
            await _unitOfWork.BeginTransaction();

            // Tạo đơn hàng
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
            decimal tongTienHang = 0;
            // Thêm chi tiết đơn hàng
            foreach (var item in model.DanhSachSanPham)
            {
                // Tự tra giá từ bảng HangHoa
                var hangHoa = await _unitOfWork.GetRepository<HangHoa>()
                    .SingleOrDefaultAsync(h => h.MaHangHoa == item.MaHangHoa);

                if (hangHoa == null)
                    throw new Exception($"Không tìm thấy hàng hóa với mã: {item.MaHangHoa}");

                if (hangHoa.GiaHangHoa == null)
                    throw new Exception($"Hàng hóa {item.MaHangHoa} chưa có giá bán.");

                var donGia = (int)hangHoa.GiaHangHoa.Value;
                var thanhTien = donGia * item.SoLuong;
                thanhTien += donGia;

                var chiTiet = new ChiTietDonHang
                {
                    MaDonHang = maDonHang,
                    MaHangHoa = item.MaHangHoa,
                    SoLuong = item.SoLuong,
                    DonGia = donGia
                };
                await _unitOfWork.GetRepository<ChiTietDonHang>().AddAsync(chiTiet);

                // Trừ kho nếu đủ hàng
                if (!hetHang.Contains(item.MaHangHoa))
                {
                    await _unitOfWork.DonHangRepository.TruTonKhoAsync(item.MaHangHoa, item.SoLuong);
                }
            }

            // Ghi trạng thái đơn hàng
            await _unitOfWork.GetRepository<LichSuTrangThaiDonHang>().AddAsync(new LichSuTrangThaiDonHang
            {
                MaLichSu = "LS" + DateTime.Now.Ticks,
                MaDonHang = maDonHang,
                MaTrangThai = maTrangThai,
                NgayCapNhat = DateTime.Now,
                GhiChu = hetHang.Count > 0 ? "Một số hàng hóa hết kho, đang xử lý" : "Đơn hàng chờ xác nhận"
            });

            // Ghi tình trạng chi tiết
            await _unitOfWork.GetRepository<TinhTrangDonHangChiTiet>().AddAsync(new TinhTrangDonHangChiTiet
            {
                MaTinhTrangChiTiet = "TTCT" + DateTime.Now.Ticks,
                MaDonHang = maDonHang,
                NoiDung = maTrangThai == "TT02" ? "Thiếu hàng , đang xử lý" : "Đang chờ xác nhận",
                ThoiGian = DateTime.Now,
                GhiChu = hetHang.Count > 0 ? "Có hàng hóa vượt tồn kho" : "Tồn kho đầy đủ"
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