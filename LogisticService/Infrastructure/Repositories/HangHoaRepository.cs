using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisticService.Models;
// public interface IHangHoaRepository 
// {
//     // Add custom methods for Entity here if needed
//     public Task<IEnumerable<HangHoa>> GetAllAsync();
//     public Task<HangHoa?> GetByIdAsync(string id);
//     public Task AddAsync(HangHoa model);
//     public Task Update(string id, HangHoa modelUpdate);
//     public void Update(HangHoa modelUpdate);
// }

// public class HangHoaRepository :  IHangHoaRepository
// {
//     private readonly LogisticDBServiceContext _context;
//     public HangHoaRepository(LogisticDBServiceContext context)
//     {
//         _context = context;
//     }
//     public async Task<IEnumerable<HangHoa>> GetAllAsync()
//     {
//         return await _context.Set<HangHoa>().ToListAsync();
//     }
//     public async Task<HangHoa?> GetByIdAsync(string id)
//     {
//         return await _context.Set<HangHoa>().FindAsync(id);
//     }
//     public async Task AddAsync(HangHoa model)
//     {
//         await _context.Set<HangHoa>().AddAsync(model);
//     }
//     public async Task Update(string id, HangHoa modelUpdate)
//     {
//         HangHoa? prod = await GetByIdAsync(id);
//         if (prod != null)
//         {
//             PropertyInfo[] lstProp = typeof(HangHoa).GetProperties();
//             foreach (PropertyInfo key in lstProp)
//             {
//                 key.SetValue(prod, modelUpdate, null);
//             }
//         }
//     }
//     public void Update(HangHoa modelUpdate)
//     {
//         _context.Set<HangHoa>().Update(modelUpdate);
//     }
//     public async Task DeleteAsync(string id)
//     {
//         var model = await GetByIdAsync(id);
//         if (model is not null)
//         {
//             _context.Set<HangHoa>().Remove(model);
//         }
//     }
// }

public interface IHangHoaRepository : IRepository<HangHoa>
{

}
public class HangHoaRepository : Repository<HangHoa>,IHangHoaRepository
{
    public HangHoaRepository(LogisticDBServiceContext context) : base(context)
    {
        
    }
}