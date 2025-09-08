using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisticService.Models;


public interface ILichSuTrangThaiDonHangRepository : IRepository<LichSuTrangThaiDonHang>
{
    
}
public class LichSuTrangThaiDonHangRepository : Repository<LichSuTrangThaiDonHang>,ILichSuTrangThaiDonHangRepository
{
    public LichSuTrangThaiDonHangRepository(LogisticDbServiceContext context) : base(context)
    {
        
    }
}