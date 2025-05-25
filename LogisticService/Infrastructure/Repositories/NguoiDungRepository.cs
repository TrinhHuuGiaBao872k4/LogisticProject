using LogisticService.Models;

public interface INguoiDungRepository : IRepository<NguoiDung>
{
    // Add custom methods for Entity here if needed
}

public class NguoiDungRepository : Repository<NguoiDung>, INguoiDungRepository
{
    public NguoiDungRepository(LogisticDbServiceContext context) : base(context)
    {
        
    }
}