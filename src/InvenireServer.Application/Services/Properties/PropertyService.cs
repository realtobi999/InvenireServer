using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Factories;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Properties;

namespace InvenireServer.Application.Services.Properties;

public class PropertyService : IPropertyService
{
    private readonly IRepositoryManager _repositories;
    private readonly IValidator<Property> _validator;

    public PropertyService(IRepositoryManager repositories, IValidatorFactory validators)
    {
        _validator = validators.Initiate<Property>();
        _repositories = repositories;

        Items = new PropertyItemService(repositories, validators.Initiate<PropertyItem>());
    }

    public IPropertyItemService Items { get; }

    public async Task<Property> GetAsync(Expression<Func<Property, bool>> predicate)
    {
        var property = await TryGetAsync(predicate);

        if (property is null) throw new NotFound404Exception($"The requested {nameof(Property).ToLower()} was not found in the system.");

        return property;
    }

    public async Task<Property?> TryGetAsync(Expression<Func<Property, bool>> predicate)
    {
        var property = await _repositories.Properties.GetAsync(predicate);

        return property;
    }

    public async Task CreateAsync(Property property)
    {
        var (valid, exception) = await _validator.ValidateAsync(property);
        if (!valid && exception is not null) throw exception;

        _repositories.Properties.Create(property);
        await _repositories.SaveOrThrowAsync();
    }
}