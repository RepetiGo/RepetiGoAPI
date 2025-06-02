
using System.Linq.Expressions;

namespace backend.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();

    }
    public virtual async Task AddAsync(T entity)
    {
        // mark the entity state as Added state
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task<ICollection<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual Task UpdateAsync(T entity)
    {
        // mark the entity state as Unchanged state first
        _dbSet.Attach(entity);

        // set the state to Modified
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual async Task<bool> TryDeleteAsync(object id)
    {
        var entityToDelete = await _dbSet.FindAsync(id);
        if (entityToDelete is null)
        {
            return false;
        }

        await TryDeleteAsync(entityToDelete);

        return true;
    }

    public virtual Task TryDeleteAsync(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            // mark the entity state as Unchanged state first
            _dbSet.Attach(entity);
        }

        // mark the entity state as Deleted state
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}