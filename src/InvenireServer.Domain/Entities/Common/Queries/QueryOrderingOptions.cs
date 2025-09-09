using System.Reflection;
using System.Linq.Expressions;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Common.Queries;

public class QueryOrderingOptions<TEntity> where TEntity : class
{
    public QueryOrderingOptions(string? order, bool? descending)
    {
        if (order is null || descending is null)
        {
            OrderBy = null;
            OrderByDescending = false;
            return;
        }

        var property = typeof(TEntity).GetProperty(order, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property is null) throw new BadRequest400Exception($"The field to order by is not valid.");

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var converted = Expression.Convert(Expression.Property(parameter, property), typeof(object));

        OrderBy = Expression.Lambda<Func<TEntity, object>>(converted, parameter);
        OrderByDescending = descending.Value;
    }

    public QueryOrderingOptions(Expression<Func<TEntity, object>> order, bool descending)
    {
        OrderBy = order;
        OrderByDescending = descending;
    }

    public bool OrderByDescending { get; set; }
    public Expression<Func<TEntity, object>>? OrderBy { get; set; }
}
