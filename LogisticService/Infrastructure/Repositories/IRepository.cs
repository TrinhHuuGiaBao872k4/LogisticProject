using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using LogisticService.Models;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(string id);
    Task AddAsync(T entity);
    void Update(T entity);
    Task DeleteAsync(string id);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly LogisticDbServiceContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(LogisticDbServiceContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return _dbSet.AsNoTracking();
    }


    public async Task<T?> GetByIdAsync(string id)
        => await _dbSet.FindAsync(id);

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task DeleteAsync(string id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AsNoTracking().SingleOrDefaultAsync(predicate);


    public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    
}



