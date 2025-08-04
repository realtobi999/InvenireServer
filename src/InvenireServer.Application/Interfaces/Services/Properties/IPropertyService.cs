using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Services.Properties.Suggestions;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Services.Properties;

public interface IPropertyService
{
    IPropertyDtoService Dto { get; }
    IPropertyItemService Items { get; }
    IPropertyScanService Scans { get; }
    IPropertySuggestionService Suggestion { get; }
    Task<Property> GetAsync(Expression<Func<Property, bool>> predicate);
    Task<Property?> TryGetAsync(Expression<Func<Property, bool>> predicate);
    Task<Property?> TryGetForAsync(Organization organization);
    Task CreateAsync(Property property);
    Task UpdateAsync(Property property);
    Task DeleteAsync(Property property);
}

public interface IPropertyDtoService
{
    Task<PropertyDto> GetAsync(Expression<Func<Property, bool>> predicate);
    Task<PropertyDto?> TryGetAsync(Expression<Func<Property, bool>> predicate);
    Task<PropertyDto?> TryGetForAsync(Admin admin);
}