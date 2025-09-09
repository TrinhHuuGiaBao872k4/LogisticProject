using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisticService.Models;


public interface IChiTietDonHangRepository : IRepository<ChiTietDonHang>
{
    
}
public class ChiTietDonHangRepository : Repository<ChiTietDonHang>,IChiTietDonHangRepository
{
    public ChiTietDonHangRepository(LogisticDbServiceContext context) : base(context)
    {
        
    }
}