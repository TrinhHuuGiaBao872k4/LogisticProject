using LogisticService.Models;
using Microsoft.AspNetCore.Http.HttpResults;

public interface IHangHoaService : IServiceBase<HangHoa>
{
    // Task<dynamic> GetAllHangHoa();
    // Task<HangHoa> GetHangHoaById(string id);
    Task<HTTPResponseClient<IEnumerable<HangHoa>>> GetAllHangHoaAsync();
    Task<HTTPResponseClient<HangHoa?>> GetHangHoaByIdAsync(string id);
    Task<IEnumerable<HangHoa>> GetAllWithNavigationPropertiesAsync();
}

public class HangHoaService : ServiceBase<HangHoa>, IHangHoaService
{
    // private readonly IHangHoaRepository _repository;
    // private readonly IUnitOfWork _uow;

    public HangHoaService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {

    }
    public async Task<HTTPResponseClient<IEnumerable<HangHoa>>> GetAllHangHoaAsync()
    {
        var res = await _repository.GetAllAsync();
        HTTPResponseClient<IEnumerable<HangHoa>> data = new HTTPResponseClient<IEnumerable<HangHoa>>()
        {
            StatusCode = 200,
            Data = res.ToList().Skip(0).Take(10),
            DateTime = DateTime.Now,
            Message = "Successfully"
        };
        return data;
    }
    public async Task<HTTPResponseClient<HangHoa?>> GetHangHoaByIdAsync(string id)
    {
        var res = await _repository.GetByIdAsync(id);
        HTTPResponseClient<HangHoa?> data = new HTTPResponseClient<HangHoa?>()
        {
            StatusCode = 200,
            Data = res,
            DateTime = DateTime.Now,
            Message = "Successfully"
        };
        return data;
    }
    public async Task<IEnumerable<HangHoa>> GetAllWithNavigationPropertiesAsync()
    {
        return await _repository.GetAllWithNavigationPropertiesAsync(h=>h.ChiTietDonHangs);
    }
}