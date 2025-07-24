using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected readonly InvenireServerContext Context;

    protected RepositoryBase(InvenireServerContext context)
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
        return await GetQueryable().FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> IndexAsync()
    {
        return await GetQueryable().ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> IndexAsync(Expression<Func<T, bool>> predicate)
    {
        return await GetQueryable().Where(predicate).ToListAsync();
    }

    protected virtual IQueryable<T> GetQueryable()
    {
        return Context.Set<T>();
    }
}