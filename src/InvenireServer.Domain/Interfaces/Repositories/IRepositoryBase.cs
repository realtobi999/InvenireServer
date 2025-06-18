using System.Linq.Expressions;

namespace InvenireServer.Domain.Interfaces.Repositories;

public interface IRepositoryBase<T> where T : class
{
    Task<IEnumerable<T>> IndexAsync();

    Task<IEnumerable<T>> IndexAsync(Expression<Func<T, bool>> predicate);

    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

    void Create(T entity);

    void Update(T entity);

    void Delete(T entity);
}