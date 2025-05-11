using LogisticService.Models;

public interface IHangHoaService
{
    
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
}