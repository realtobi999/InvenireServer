using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

public interface IPropertyItemRepository : IRepositoryBase<PropertyItem>
{
    Expression<Func<PropertyItem, bool>> BuildSearchExpression(string term);
}
