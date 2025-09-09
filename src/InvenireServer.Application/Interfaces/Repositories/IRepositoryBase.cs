using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Interfaces.Repositories;

public interface IRepositoryBase<TEntity> where TEntity : class
{
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<int> CountAsync(IEnumerable<Expression<Func<TEntity, bool>>> predicates);
    Task<IEnumerable<TEntity>> IndexAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TResult>> IndexAsync<TResult>(QueryOptions<TEntity, TResult> options);
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TResult?> GetAsync<TResult>(QueryOptions<TEntity, TResult> options);
    void Create(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}