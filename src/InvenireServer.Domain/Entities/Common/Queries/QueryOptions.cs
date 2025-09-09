using System.Linq.Expressions;

namespace InvenireServer.Domain.Entities.Common.Queries;

public class QueryOptions<TEntity, TResult> where TEntity : class
{
    public QueryPaginationOptions? Pagination { get; set; }
    public QueryOrderingOptions<TEntity>? Ordering { get; set; }
    public QueryFilteringOptions<TEntity>? Filtering { get; set; }
    public Expression<Func<TEntity, TResult>>? Selector { get; set; }
}
