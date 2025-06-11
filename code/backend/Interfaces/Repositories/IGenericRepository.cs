using System.Linq.Expressions;

namespace backend.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "");
        Task<T?> GetByIdAsync(int id);
        Task<bool> TryDeleteAsync(object id);
        Task UpdateAsync(T entity);
    }
}