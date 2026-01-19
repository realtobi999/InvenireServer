using System.Linq.Expressions;

namespace InvenireServer.Domain.Entities.Common.Queries;

/// <summary>
/// Represents filtering options for queries.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public class QueryFilteringOptions<TEntity> where TEntity : class
{
    public List<Expression<Func<TEntity, bool>>?> Filters { get; set; } = [];
}
