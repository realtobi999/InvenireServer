using System.Linq.Expressions;
using InvenireServer.Domain.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly InvenireServerContext Context;

    protected BaseRepository(InvenireServerContext context)
    {
        Context = context;
    }

    public virtual void Create(T entity)
    {
        Context.Set<T>().Add(entity);
    }

    public virtual void Delete(T entity)
    {
        Context.Set<T>().Remove(entity);
    }

    public virtual void Update(T entity)
    {
        Context.Set<T>().Update(entity);
    }

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> IndexAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> IndexAsync(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().Where(predicate).ToListAsync();
    }
}