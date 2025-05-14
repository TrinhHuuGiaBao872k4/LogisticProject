// unitofwork
using LogisticService.Models;

public interface IUnitOfWork : IAsyncDisposable
{
    public IHangHoaRepository _hangHoaRepository{get;}
    Task<int> SaveChangesAsync();
}

public class UnitOfWork: IUnitOfWork
{
    public IHangHoaRepository _hangHoaRepository{get;}

    private readonly LogisticDbServiceContext _context;
    
    public UnitOfWork(LogisticDbServiceContext context, IHangHoaRepository hangHoaRepository)
    {
        _context = context;
        _hangHoaRepository = hangHoaRepository;
    }
    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}


