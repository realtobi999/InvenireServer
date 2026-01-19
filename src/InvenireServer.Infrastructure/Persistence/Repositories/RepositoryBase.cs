using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Repositories;
using InvenireServer.Domain.Entities.Common.Queries;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides base repository operations for an entity.
/// </summary>
/// <typeparam name="Entity">Entity type.</typeparam>
public abstract class RepositoryBase<Entity> : IRepositoryBase<Entity> where Entity : class
{
    protected readonly InvenireServerContext Context;

    protected RepositoryBase(InvenireServerContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Adds an entity to the repository.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    public virtual void Create(Entity entity)
    {
        Context.Set<Entity>().Add(entity);
    }

    /// <summary>
    /// Adds an entity and persists the change.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    /// <returns>Awaitable task representing the create operation.</returns>
    public virtual async Task ExecuteCreateAsync(Entity entity)
    {
        Create(entity);
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an entity in the repository.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    public virtual void Update(Entity entity)
    {
        Context.Set<Entity>().Update(entity);
    }

    /// <summary>
    /// Updates multiple entities in the repository.
    /// </summary>
    /// <param name="entities">Entities to update.</param>
    public void Update(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Update(entity);
    }

    /// <summary>
    /// Updates an entity and persists the change.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    /// <returns>Awaitable task representing the update operation.</returns>
    public virtual async Task ExecuteUpdateAsync(Entity entity)
    {
        Update(entity);
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">Entity to delete.</param>
    public virtual void Delete(Entity entity)
    {
        Context.Set<Entity>().Remove(entity);
    }

    /// <summary>
    /// Deletes an entity and persists the change.
    /// </summary>
    /// <param name="entity">Entity to delete.</param>
    /// <returns>Awaitable task representing the delete operation.</returns>
    public virtual async Task ExecuteDeleteAsync(Entity entity)
    {
        Delete(entity);
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes entities matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <returns>Awaitable task representing the delete operation.</returns>
    public virtual async Task ExecuteDeleteWhereAsync(Expression<Func<Entity, bool>> predicate)
    {
        await Context.Set<Entity>().Where(predicate).ExecuteDeleteAsync();
    }

    /// <summary>
    /// Returns the first entity matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <returns>Awaitable task returning the entity or null.</returns>
    public virtual async Task<Entity?> GetAsync(Expression<Func<Entity, bool>> predicate)
    {
        return await GetQueryable().FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Returns an entity using query options.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="options">Query options.</param>
    /// <returns>Awaitable task returning the entity or null.</returns>
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

    /// <summary>
    /// Returns entities matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <returns>Awaitable task returning matching entities.</returns>
    public virtual async Task<IEnumerable<Entity>> IndexAsync(Expression<Func<Entity, bool>> predicate)
    {
        return await GetQueryable().Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Returns entities using query options.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="options">Query options.</param>
    /// <returns>Awaitable task returning the query results.</returns>
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

    /// <summary>
    /// Counts entities matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <returns>Awaitable task returning the number of matching entities.</returns>
    public virtual async Task<int> CountAsync(Expression<Func<Entity, bool>> predicate)
    {
        return await Context.Set<Entity>()
            .AsNoTracking()
            .Where(predicate)
            .CountAsync();
    }

    /// <summary>
    /// Counts entities matching all provided predicates.
    /// </summary>
    /// <param name="predicates">Filter predicates.</param>
    /// <returns>Awaitable task returning the number of matching entities.</returns>
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

    /// <summary>
    /// Returns the base queryable for the entity.
    /// </summary>
    /// <returns>Queryable for the entity.</returns>
    protected virtual IQueryable<Entity> GetQueryable()
    {
        return Context.Set<Entity>();
    }
}
