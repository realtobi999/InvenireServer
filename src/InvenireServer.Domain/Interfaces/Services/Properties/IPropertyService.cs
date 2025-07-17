using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Interfaces.Services.Properties.Suggestions;

namespace InvenireServer.Domain.Interfaces.Services.Properties;

public interface IPropertyService
{
    IPropertyItemService Items { get; }
    IPropertyScanService Scans { get; }
    IPropertySuggestionService Suggestion { get; }
    Task<Property> GetAsync(Expression<Func<Property, bool>> predicate);
    Task<Property?> TryGetAsync(Expression<Func<Property, bool>> predicate);
    Task<Property?> TryGetForAsync(Organization organization);
    Task CreateAsync(Property property);
    Task UpdateAsync(Property property);
}