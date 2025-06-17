using System.Linq.Expressions;

namespace RepetiGo.Api.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<ICollection<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            PaginationQuery? paginationQuery = null);
        Task<T?> GetByIdAsync(int id);
        Task<bool> TryDeleteAsync(int id);
        Task TryDeleteAsync(T entity);
        Task UpdateAsync(T entity);
    }
}