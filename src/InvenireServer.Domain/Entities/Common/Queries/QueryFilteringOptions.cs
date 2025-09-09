using System.Linq.Expressions;

namespace InvenireServer.Domain.Entities.Common.Queries;

public class QueryFilteringOptions<TEntity> where TEntity : class
{
    public List<Expression<Func<TEntity, bool>>?> Filters { get; set; } = [];
}
