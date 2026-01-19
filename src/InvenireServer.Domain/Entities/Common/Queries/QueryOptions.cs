using System.Linq.Expressions;

namespace InvenireServer.Domain.Entities.Common.Queries;

/// <summary>
/// Represents options for querying entities.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TResult">Result type.</typeparam>
public class QueryOptions<TEntity, TResult> where TEntity : class
{
    public QueryPaginationOptions? Pagination { get; set; }
    public QueryOrderingOptions<TEntity>? Ordering { get; set; }
    public QueryFilteringOptions<TEntity>? Filtering { get; set; }
    public Expression<Func<TEntity, TResult>>? Selector { get; set; }
}
