using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisticService.Models;


public interface ITinhTrangDonHangChiTietRepository : IRepository<TinhTrangDonHangChiTiet>
{
    
}
public class TinhTrangDonHangChiTietRepository : Repository<TinhTrangDonHangChiTiet>,ITinhTrangDonHangChiTietRepository
{
    public TinhTrangDonHangChiTietRepository(LogisticDbServiceContext context) : base(context)
    {
        
    }
}