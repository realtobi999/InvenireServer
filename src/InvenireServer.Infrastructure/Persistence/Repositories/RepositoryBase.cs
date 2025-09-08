using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using InvenireServer.Application.Interfaces.Repositories;
using InvenireServer.Domain.Entities.Common;
using System.Net.Quic;

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

    public virtual async Task<TResult?> GetAsync<TResult>(Expression<Func<Entity, bool>> predicate, QueryOptions<Entity, TResult> options)
    {
        var query = GetQueryable();

        query = query.Where(predicate);

        if (options.OrderBy is not null)
            query = options.OrderByDescending ? query.OrderByDescending(options.OrderBy) : query.OrderBy(options.OrderBy);

        if (options.Pagination is not null)
            query = query.Skip(options.Pagination.Offset).Take(options.Pagination.Limit);

        if (options.Selector is not null)
        {
            return await query.Select(options.Selector).FirstOrDefaultAsync();
        }
        else
        {
            if (typeof(TResult) != typeof(Entity)) throw new InvalidOperationException("The selector for the query must be provided.");

            return (TResult?)(object?)await query.FirstOrDefaultAsync();
        }
    }

    public virtual async Task<IEnumerable<Entity>> IndexAsync(Expression<Func<Entity, bool>> predicate)
    {
        return await GetQueryable().Where(predicate).ToListAsync();
    }

    public virtual async Task<IEnumerable<TResult>> IndexAsync<TResult>(Expression<Func<Entity, bool>> predicate, QueryOptions<Entity, TResult> options)
    {
        var query = GetQueryable();

        query = query.Where(predicate);

        if (options.OrderBy is not null)
            query = options.OrderByDescending ? query.OrderByDescending(options.OrderBy) : query.OrderBy(options.OrderBy);

        if (options.Pagination is not null)
            query = query.Skip(options.Pagination.Offset).Take(options.Pagination.Limit);

        if (options.Selector is not null)
        {
            return await query.Select(options.Selector).ToListAsync();
        }
        else
        {
            if (typeof(TResult) != typeof(Entity)) throw new InvalidOperationException("The selector for the query must be provided.");

            return (IEnumerable<TResult>)(object)await query.ToListAsync();
        }
    }

    public async Task<int> CountAsync(Expression<Func<Entity, bool>> predicate)
    {
        return await Context.Set<Entity>()
            .AsNoTracking()
            .Where(predicate)
            .CountAsync();
    }

    protected virtual IQueryable<Entity> GetQueryable()
    {
        return Context.Set<Entity>();
    }
}