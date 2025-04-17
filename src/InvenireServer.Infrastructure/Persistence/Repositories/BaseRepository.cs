using System.Linq.Expressions;
using InvenireServer.Domain.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly InvenireServerContext _context;

    protected BaseRepository(InvenireServerContext context)
    {
        _context = context;
    }

    public virtual void Create(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public virtual void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public virtual void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> IndexAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> IndexAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }
}