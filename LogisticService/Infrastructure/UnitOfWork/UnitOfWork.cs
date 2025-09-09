// unitofwork
using LogisticService.Models;

public interface IUnitOfWork : IAsyncDisposable
{
    public IDonHangRepository _donHangRepository { get; }
    public IHangHoaRepository _hangHoaRepository { get; }
    public INguoiDungRepository _nguoiDungRepository { get; }
    public ILichSuTrangThaiDonHangRepository _lichSuTrangThaiDonHangRepository {get; }
    public ITinhTrangDonHangChiTietRepository _tinhTrangDonHangChiTietRepository {get; }
    public IChiTietDonHangRepository _chiTietDonHangRepository { get;  }

    Task BeginTransaction();
    IRepository<T> GetRepository<T>() where T : class;
    Task<int> SaveChangesAsync();
    Task CommitTransaction();
    Task RollBack();
}

public class UnitOfWork : IUnitOfWork
{
    public IDonHangRepository _donHangRepository { get; }
    public IHangHoaRepository _hangHoaRepository { get; }
    public INguoiDungRepository _nguoiDungRepository { get; }
    public ILichSuTrangThaiDonHangRepository _lichSuTrangThaiDonHangRepository {get; }
    public ITinhTrangDonHangChiTietRepository _tinhTrangDonHangChiTietRepository {get; }
    public IChiTietDonHangRepository _chiTietDonHangRepository { get; }


    private readonly LogisticDbServiceContext _context;

    public UnitOfWork(LogisticDbServiceContext context, IHangHoaRepository hangHoaRepository, IDonHangRepository donHangRepository, INguoiDungRepository nguoiDungRepository, ILichSuTrangThaiDonHangRepository lichSuTrangThaiDonHangRepository, ITinhTrangDonHangChiTietRepository tinhTrangDonHangChiTietRepository, IChiTietDonHangRepository chiTietDonHangRepository)
    {
        _context = context;
        _hangHoaRepository = hangHoaRepository;
        _donHangRepository = donHangRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _lichSuTrangThaiDonHangRepository = lichSuTrangThaiDonHangRepository;
        _tinhTrangDonHangChiTietRepository = tinhTrangDonHangChiTietRepository;
        _chiTietDonHangRepository = chiTietDonHangRepository;
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        if (typeof(T) == typeof(HangHoa))
        {
            return (IRepository<T>)_hangHoaRepository;
        }
        if (typeof(T) == typeof(DonHang))
        {
            return (IRepository<T>)_donHangRepository;
        }
        if (typeof(T) == typeof(NguoiDung))
        {
            return (IRepository<T>)_nguoiDungRepository;
        }
        if (typeof(T) == typeof(LichSuTrangThaiDonHang))
        {
            return (IRepository<T>)_lichSuTrangThaiDonHangRepository;
        }
        if (typeof(T) == typeof(TinhTrangDonHangChiTiet))
        {
            return (IRepository<T>)_tinhTrangDonHangChiTietRepository;
        }
        if (typeof(T) == typeof(ChiTietDonHang))
        {
            return (IRepository<T>)_chiTietDonHangRepository;
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


