using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Interfaces.Repositories;

public interface IRepositoryBase<Entity> where Entity : class
{
    Task<int> CountAsync(Expression<Func<Entity, bool>> predicate);
    Task<IEnumerable<Entity>> IndexAsync(Expression<Func<Entity, bool>> predicate);
    Task<IEnumerable<EntityDto>> IndexAndProjectAsync<EntityDto>(Expression<Func<Entity, bool>> predicate, Expression<Func<Entity, EntityDto>> selector, PaginationParameters pagination);
    Task<Entity?> GetAsync(Expression<Func<Entity, bool>> predicate);
    Task<EntityDto?> GetAndProjectAsync<EntityDto>(Expression<Func<Entity, bool>> predicate, Expression<Func<Entity, EntityDto>> selector);
    void Create(Entity entity);
    void Update(Entity entity);
    void Delete(Entity entity);
}