using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
using RPM_Project_Backend.Services.Database;

namespace RPM_Project_Backend.Repositories;

public class BaseRepository<TModel> : IBaseRepository<TModel> where TModel : BaseModel
{
    private readonly ApplicationContext _context;
    private readonly DbSet<TModel> _dbSet;

    public BaseRepository(ApplicationContext context)
    {
        _context = context;
        _dbSet = context.Set<TModel>();
    }

    public async Task<TModel> CreateAsync(TModel model)
    {
        await _dbSet.AddAsync(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<TModel> DeleteAsync(long id)
    {
        var toDelete = await _dbSet.FindAsync(id);
        var entityEntry = _dbSet.Remove(toDelete!);
        await _context.SaveChangesAsync();
        return entityEntry.Entity;
    }
    
    public async Task<TModel> DeleteAsync(TModel item)
    {
        var entityEntry = _dbSet.Remove(item);
        await _context.SaveChangesAsync();
        return entityEntry.Entity;
    }

    public async Task<TModel> UpdateAsync(TModel model)
    {
        _context.Entry(model).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return model;
    }
    
    public IEnumerable<TModel> GetWithInclude(params Expression<Func<TModel, object>>[] includeProperties)
    {
        return Include(includeProperties).ToList();
    }
 
    public IEnumerable<TModel> GetWithInclude(Func<TModel,bool> predicate, 
        params Expression<Func<TModel, object>>[] includeProperties)
    {
        var query = Include(includeProperties);
        return query.AsEnumerable().Where(predicate).ToList();
    }

    public IQueryable<TModel> Include(params Expression<Func<TModel, object>>[] includeProperties)
    {
        IQueryable<TModel> query = _dbSet.AsNoTracking();

        return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    public async Task<IEnumerable<TModel>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync()!;
    
    public async Task<IEnumerable<TModel>> GetWhereAsync(Func<TModel, bool> predicate) =>
        await _dbSet.AsNoTracking().AsEnumerable().Where(predicate).AsQueryable().ToListAsync()!; //TODO я нихера не знаю как это должно работать, сделал так чтобы ошибок не высвечивалось)

    public async Task<TModel> GetAsync(long id) => (await _dbSet.FindAsync(id))!;
}