using LogisticService.Models;
using LogisticService.PasswordHelper;
using LogisticService.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public interface IChiTietDonHangService : IServiceBase<ChiTietDonHang>
{
    Task<IEnumerable<ChiTietDonHangVM>> GetByDonHangAsync(string maDonHang);
}

public class ChiTietDonHangService : ServiceBase<ChiTietDonHang>, IChiTietDonHangService
{
    private readonly JwtAuthService _JwtAuthService;

    public ChiTietDonHangService(IUnitOfWork unitOfWork, JwtAuthService jwtAuthService) : base(unitOfWork)
    {
        _JwtAuthService = jwtAuthService;
    }
    public async Task<IEnumerable<ChiTietDonHangVM>> GetByDonHangAsync(string maDonHang)
    {
        // Lấy dữ liệu và include navigation property MaHangHoaNavigation
        var chiTietDonHangs = await _uow._chiTietDonHangRepository
            .GetAllWithNavigationPropertiesAsync(ct => ct.MaHangHoaNavigation);

        var filtered = chiTietDonHangs
            .Where(ct => ct.MaDonHang == maDonHang)
            .Select(ct => new ChiTietDonHangVM
            {
                MaChiTietDonHang = ct.MaChiTietDonHang,
                MaHangHoa = ct.MaHangHoa,
                TenHangHoa = ct.MaHangHoaNavigation.TenHangHoa,
                SoLuong = ct.SoLuong ?? 0,
                DonGia = ct.DonGia ?? 0
            });

        return filtered;
    }
}