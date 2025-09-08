using LogisticService.Models;
using LogisticService.PasswordHelper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public interface ITinhTrangDonHangChiTietService : IServiceBase<TinhTrangDonHangChiTiet>
{

}

public class TinhTrangDonHangChiTietService : ServiceBase<TinhTrangDonHangChiTiet>,ITinhTrangDonHangChiTietService
{
    private readonly JwtAuthService _JwtAuthService;

    public TinhTrangDonHangChiTietService(IUnitOfWork unitOfWork, JwtAuthService jwtAuthService) : base(unitOfWork)
    {
        _JwtAuthService = jwtAuthService;
    }

}