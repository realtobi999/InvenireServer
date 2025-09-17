using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Application.Interfaces.Repositories;

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

    public virtual async Task ExecuteCreateAsync(Entity entity)
    {
        Create(entity);
        await Context.SaveChangesAsync();
    }

    public virtual void Update(Entity entity)
    {
        Context.Set<Entity>().Update(entity);
    }

    public virtual async Task ExecuteUpdateAsync(Entity entity)
    {
        Update(entity);
        await Context.SaveChangesAsync();
    }

    public virtual void Delete(Entity entity)
    {
        Context.Set<Entity>().Remove(entity);
    }

    public virtual async Task ExecuteDeleteAsync(Entity entity)
    {
        Delete(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task ExecuteDeleteWhereAsync(Expression<Func<Entity, bool>> predicate)
    {
        Console.WriteLine(await Context.Set<Entity>().Where(predicate).ExecuteDeleteAsync());
    }

    public virtual async Task<Entity?> GetAsync(Expression<Func<Entity, bool>> predicate)
    {
        return await GetQueryable().FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<TResult?> GetAsync<TResult>(QueryOptions<Entity, TResult> options)
    {
        var query = GetQueryable().AsNoTracking();

        if (options.Filtering is not null)
            foreach (var filter in options.Filtering.Filters)
            {
                if (filter is null) continue;

                query = query.Where(filter);
            }

        if (options.Ordering is not null && options.Ordering.OrderBy is not null)
            query = options.Ordering.OrderByDescending ? query.OrderByDescending(options.Ordering.OrderBy) : query.OrderBy(options.Ordering.OrderBy);

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

    public virtual async Task<IEnumerable<TResult>> IndexAsync<TResult>(QueryOptions<Entity, TResult> options)
    {
        var query = GetQueryable().AsNoTracking();

        if (options.Filtering is not null)
            foreach (var filter in options.Filtering.Filters)
            {
                if (filter is null) continue;

                query = query.Where(filter);
            }

        if (options.Ordering is not null && options.Ordering.OrderBy is not null)
            query = options.Ordering.OrderByDescending ? query.OrderByDescending(options.Ordering.OrderBy) : query.OrderBy(options.Ordering.OrderBy);

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

    public virtual async Task<int> CountAsync(Expression<Func<Entity, bool>> predicate)
    {
        return await Context.Set<Entity>()
            .AsNoTracking()
            .Where(predicate)
            .CountAsync();
    }

    public virtual async Task<int> CountAsync(IEnumerable<Expression<Func<Entity, bool>>> predicates)
    {
        var query = GetQueryable().AsNoTracking();

        foreach (var filter in predicates)
        {
            if (filter is null) continue;

            query = query.Where(filter);
        }

        return await query.CountAsync();
    }

    protected virtual IQueryable<Entity> GetQueryable()
    {
        return Context.Set<Entity>();
    }

}
