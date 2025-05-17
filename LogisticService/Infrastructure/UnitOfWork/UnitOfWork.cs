// unitofwork
using LogisticService.Models;

public interface IUnitOfWork : IAsyncDisposable
{
    public IHangHoaRepository _hangHoaRepository { get; }
    IRepository<T> GetRepository<T>() where T : class;
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    public IHangHoaRepository _hangHoaRepository { get; }

    private readonly LogisticDBServiceContext _context;

    public UnitOfWork(LogisticDBServiceContext context, IHangHoaRepository hangHoaRepository)
    {
        _context = context;
        _hangHoaRepository = hangHoaRepository;
    }
    public IRepository<T> GetRepository<T>() where T : class
    {
        if (typeof(T) == typeof(HangHoa))
        {
            return (IRepository<T>)_hangHoaRepository;
        }

        throw new NotSupportedException($"No repository found for type {typeof(T).Name}");
    }
    public async Task BeginTransaction()
    {
        await _context.Database.BeginTransactionAsync();
    }
    public async Task CommitTransaction()
    {
        await _context.Database.CommitTransactionAsync();
    }
    public async Task RollBack()
    {
        await _context.Database.RollbackTransactionAsync();
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


