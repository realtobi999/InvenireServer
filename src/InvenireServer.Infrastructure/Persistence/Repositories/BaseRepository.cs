using System.Linq.Expressions;
using InvenireServer.Domain.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides a base implementation of repository operations for a specific entity type.
/// </summary>
/// <typeparam name="T">The entity type managed by the repository.</typeparam>
public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    /// <summary>
    /// The database context used to access the entity set.
    /// </summary>
    protected readonly InvenireServerContext Context;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
    /// </summary>
    /// <param name="context">The database context used for entity operations.</param>
    protected BaseRepository(InvenireServerContext context)
    {
        Context = context;
    }

    /// <inheritdoc/>
    public virtual void Create(T entity)
    {
        Context.Set<T>().Add(entity);
    }

    /// <inheritdoc/>
    public virtual void Delete(T entity)
    {
        Context.Set<T>().Remove(entity);
    }

    /// <inheritdoc/>
    public virtual void Update(T entity)
    {
        Context.Set<T>().Update(entity);
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<T>> IndexAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<T>> IndexAsync(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().Where(predicate).ToListAsync();
    }
}
