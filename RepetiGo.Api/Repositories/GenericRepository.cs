using System.Linq.Expressions;

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
            Query? query = null)
        {
            IQueryable<T> queryable = _dbSet;

            // Apply base filter if provided in the query
            if (filter is not null)
            {
                queryable = queryable.Where(filter);
            }

            // Apply dynamic filter if provided in the query
            if (query?.Filter is not null)
            {
                var dynamicFilter = DynamicQueryBuilder.BuildFilter<T>(query.Filter);
                if (dynamicFilter is not null)
                {
                    queryable = queryable.Where(dynamicFilter);
                }
            }

            // Apply includes if provided
            if (query is not null && !string.IsNullOrEmpty(query.Include))
            {
                foreach (var includeProperty in query.Include.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }
            }

            // Apply ordering if provided
            if (query?.Sort is not null)
            {
                var sortBy = DynamicQueryBuilder.BuildSortBy<T>(query.Sort, query.Descending);
                if (sortBy is not null)
                {
                    queryable = sortBy(queryable);
                }
            }

            // Apply pagination
            if (query is not null)
            {
                queryable = queryable.Skip(query.Skip + (query.Page - 1) * query.Size).Take(query.Size);
            }

            return await queryable.ToListAsync();
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