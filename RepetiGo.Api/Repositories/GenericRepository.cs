using System.Linq.Expressions;

using RepetiGo.Api.Data;
using RepetiGo.Api.Helpers;
using RepetiGo.Api.Interfaces.Repositories;

namespace RepetiGo.Api.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();

        }

        public virtual async Task<ICollection<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            PaginationQuery? paginationQuery = null)
        {
            IQueryable<T> query = _dbSet;

            // Apply filter if provided
            if (filter is not null)
            {
                query = query.Where(filter);
            }

            // Apply includes if provided
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            // Apply ordering if provided
            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            // Apply pagination
            paginationQuery ??= new PaginationQuery();
            query = query.Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
                         .Take(paginationQuery.PageSize);

            return await query.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task AddAsync(T entity)
        {
            // mark the entity state as Added state
            await _dbSet.AddAsync(entity);
        }

        public virtual Task UpdateAsync(T entity)
        {
            // update the entity as Modified
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<bool> TryDeleteAsync(int id)
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
            // mark the entity state as Deleted state
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
    }
}