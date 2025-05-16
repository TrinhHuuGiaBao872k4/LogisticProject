using LogisticService.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using LogisticService.Models.ViewModel;

public interface IHangHoaService
{
    Task<dynamic> GetAllHangHoa();
    Task<HangHoa> GetHangHoaById(string id);
}

public class HangHoaService : IHangHoaService
{
    private readonly IHangHoaRepository _repository;
    private readonly IUnitOfWork _uow;

    public HangHoaService(IHangHoaRepository hangHoaRepository, IUnitOfWork unitOfWork)
    {
        _repository = hangHoaRepository;
        _uow = unitOfWork;
    }
    public async Task<dynamic> GetAllHangHoa(){
        var res = await _uow._hangHoaRepository.GetAllAsync();
        return new {
            StatusCode = 200,
            Data = res.Skip(0).Take(1000)
        };
    }
    public async Task<HangHoa> GetHangHoaById(string id){
        var res = await _uow._hangHoaRepository.GetByIdAsync(id);
        return new ApiResponse<HangHoa>(){
            StatusCode =200,
            Data = res
        };
    }
}