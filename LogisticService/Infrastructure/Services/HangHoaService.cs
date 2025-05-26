using LogisticService.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public interface IHangHoaService : IServiceBase<HangHoa>
{
    // Task<dynamic> GetAllHangHoa();
    // Task<HangHoa> GetHangHoaById(string id);
    Task<HTTPResponseClient<IEnumerable<HangHoa>>> GetAllHangHoaAsync();
    Task<HTTPResponseClient<HangHoa?>> GetHangHoaByIdAsync(string id);
    Task<IEnumerable<HangHoa>> GetAllWithNavigationPropertiesAsync();
    Task<string> GenerateMaHangHoaAsync();
    Task<int> CountByDateAsync(DateTime date);
}

public class HangHoaService : ServiceBase<HangHoa>, IHangHoaService
{
    // private readonly IHangHoaRepository _repository;
    // private readonly IUnitOfWork _uow;
    private readonly LogisticDbServiceContext _context;

    public HangHoaService(IUnitOfWork unitOfWork, LogisticDbServiceContext context) : base(unitOfWork)
    {
        _context = context;
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
        return await _repository.GetAllWithNavigationPropertiesAsync(h => h.ChiTietDonHangs);
    }
    public async Task<int> CountByDateAsync(DateTime date)
    {
        return await _context.HangHoas
            .Where(h => h.NgaySanXuat.HasValue && h.NgaySanXuat.Value.Date == date.Date)
            .CountAsync();
    }

    public async Task<string> GenerateMaHangHoaAsync()
    {
        string prefix = "HH";
        string datePart = DateTime.Now.ToString("yyyyMMdd");

        int countToday = await CountByDateAsync(DateTime.Today);

        string ma = $"{prefix}{datePart}{(countToday + 1).ToString("D4")}"; // Ví dụ: HH202405260001
        return ma;
    }
}