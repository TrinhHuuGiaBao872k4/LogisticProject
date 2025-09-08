using LogisticService.Models;
using LogisticService.PasswordHelper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public interface IChiTietDonHangService : IServiceBase<ChiTietDonHang>
{
    Task<IEnumerable<ChiTietDonHang>> GetByDonHangAsync(string maDonHang);
}

public class ChiTietDonHangService : ServiceBase<ChiTietDonHang>,IChiTietDonHangService
{
    private readonly JwtAuthService _JwtAuthService;

    public ChiTietDonHangService(IUnitOfWork unitOfWork, JwtAuthService jwtAuthService) : base(unitOfWork)
    {
        _JwtAuthService = jwtAuthService;
    }
    public async Task<IEnumerable<ChiTietDonHang>> GetByDonHangAsync(string maDonHang)
    {
        return await _uow.GetRepository<ChiTietDonHang>()
                         .WhereAsync(ct => ct.MaDonHang == maDonHang);
    }
}