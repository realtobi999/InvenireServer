using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

public interface IPropertyItemRepository : IRepositoryBase<PropertyItem>
{
    Task ScanAsync(PropertyItem item, PropertyScan scan);
    Expression<Func<PropertyItem, bool>> BuildSearchExpression(string term);
}
