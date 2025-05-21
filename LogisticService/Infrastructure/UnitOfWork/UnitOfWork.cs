// unitofwork
using LogisticService.Models;

public interface IUnitOfWork : IAsyncDisposable
{
    IDonHangRepository DonHangRepository { get; }
    public IHangHoaRepository _hangHoaRepository { get; }

    Task BeginTransaction();
    IRepository<T> GetRepository<T>() where T : class;
    Task<int> SaveChangesAsync();
    Task CommitTransaction();
    Task RollBack();
}

public class UnitOfWork : IUnitOfWork
{
    public IDonHangRepository DonHangRepository { get; }
    public IHangHoaRepository _hangHoaRepository { get; }

    private readonly LogisticDbServiceContext _context;

    public UnitOfWork(LogisticDbServiceContext context, IHangHoaRepository hangHoaRepository, IDonHangRepository donHangRepository)
    {
        _context = context;
        _hangHoaRepository = hangHoaRepository;
        _context = context;
        _hangHoaRepository = hangHoaRepository;
        DonHangRepository = donHangRepository;
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        if (typeof(T) == typeof(HangHoa))
        {
            return (IRepository<T>)_hangHoaRepository;
        }
        if (typeof(T) == typeof(ChiTietDonHang))
        {
            return new Repository<T>(_context);
        }
        if (typeof(T) == typeof(LichSuTrangThaiDonHang))
        {
            return new Repository<T>(_context);
        }
        if (typeof(T) == typeof(TinhTrangDonHangChiTiet))
        {
            return new Repository<T>(_context);
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


