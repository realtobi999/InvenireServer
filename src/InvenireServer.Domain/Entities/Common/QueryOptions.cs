using System.Linq.Expressions;

namespace InvenireServer.Domain.Entities.Common;

public class QueryOptions<TEntity, TResult> where TEntity : class
{
    public bool OrderByDescending { get; set; } = false;
    public PaginationOptions? Pagination { get; set; }
    public Expression<Func<TEntity, object>>? OrderBy { get; set; }
    public Expression<Func<TEntity, TResult>>? Selector { get; set; }
}
