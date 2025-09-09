using LogisticService.Models;
using LogisticService.PasswordHelper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public interface ILichSuTrangThaiDonHangService : IServiceBase<LichSuTrangThaiDonHang>
{

}

public class LichSuTrangThaiDonHangService : ServiceBase<LichSuTrangThaiDonHang>,ILichSuTrangThaiDonHangService
{
    private readonly JwtAuthService _JwtAuthService;

    public LichSuTrangThaiDonHangService(IUnitOfWork unitOfWork, JwtAuthService jwtAuthService) : base(unitOfWork)
    {
        _JwtAuthService = jwtAuthService;
    }

}