using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Services.Properties;

public interface IPropertyService
{
    IPropertyItemService Items { get; }
    Task<Property> GetAsync(Expression<Func<Property, bool>> predicate);
    Task CreateAsync(Property property);
}
