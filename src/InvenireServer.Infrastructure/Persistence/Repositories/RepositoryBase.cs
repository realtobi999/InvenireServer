using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using InvenireServer.Application.Interfaces.Repositories;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<Entity> : IRepositoryBase<Entity> where Entity : class
{
    protected readonly InvenireServerContext Context;

    protected RepositoryBase(InvenireServerContext context)
    {
        Context = context;
    }

    public virtual void Create(Entity entity)
    {
        Context.Set<Entity>().Add(entity);
    }

    public virtual void Delete(Entity entity)
    {
        Context.Set<Entity>().Remove(entity);
    }

    public virtual void Update(Entity entity)
    {
        Context.Set<Entity>().Update(entity);
    }

    public virtual async Task<Entity?> GetAsync(Expression<Func<Entity, bool>> predicate)
    {
        return await GetQueryable().FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<EntityDto?> GetAndProjectToAsync<EntityDto>(Expression<Func<Entity, bool>> predicate, Expression<Func<Entity, EntityDto>> selector)
    {
        return await Context.Set<Entity>()
            .AsNoTracking()
            .Where(predicate)
            .Select(selector)
            .FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<Entity>> IndexAsync(Expression<Func<Entity, bool>> predicate)
    {
        return await GetQueryable().Where(predicate).ToListAsync();
    }

    public virtual async Task<IEnumerable<EntityDto>> IndexAndProjectToAsync<EntityDto>(Expression<Func<Entity, bool>> predicate, Expression<Func<Entity, EntityDto>> selector, PaginationParameters pagination)
    {
        return await Context.Set<Entity>()
            .AsNoTracking()
            .Where(predicate)
            .Skip(pagination.Offset)
            .Take(pagination.Limit)
            .Select(selector)
            .ToListAsync();
    }

    protected virtual IQueryable<Entity> GetQueryable()
    {
        return Context.Set<Entity>();
    }
}