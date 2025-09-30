using System.Linq.Expressions;

namespace InvenireServer.Application.Extensions;

public static class ExpressionSelectorExtensions
{
    public static Expression<Func<TSource, TDest>> Extend<TSource, TDest>(this Expression<Func<TSource, TDest>> coreSelector, params (string PropertyName, LambdaExpression ValueExpression)[] extraBindings)
    {
        var param = coreSelector.Parameters[0];

        var bindings = ((MemberInitExpression)coreSelector.Body).Bindings.ToList();

        foreach (var (propertyName, valueExpression) in extraBindings)
        {
            bindings.Add(Expression.Bind(typeof(TDest).GetProperty(propertyName)!, Expression.Invoke(valueExpression, param)));
        }

        var body = Expression.MemberInit(Expression.New(typeof(TDest)), bindings);
        return Expression.Lambda<Func<TSource, TDest>>(body, param);
    }
}
