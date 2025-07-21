using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Services.Properties.Suggestions;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Properties;
using InvenireServer.Domain.Interfaces.Services.Properties.Suggestions;
using InvenireServer.Domain.Validators.Properties;

namespace InvenireServer.Application.Services.Properties;

public class PropertyService : IPropertyService
{
    private readonly IRepositoryManager _repositories;

    public PropertyService(IRepositoryManager repositories)
    {
        _repositories = repositories;

        Items = new PropertyItemService(_repositories);
        Scans = new PropertyScanService(_repositories);
        Suggestion = new PropertySuggestionService(_repositories);
    }

    public IPropertyItemService Items { get; }
    public IPropertyScanService Scans { get; }
    public IPropertySuggestionService Suggestion { get; }


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

    public async Task<Property?> TryGetForAsync(Organization organization)
    {
        var property = await TryGetAsync(p => p.OrganizationId == organization.Id);

        return property;
    }

    public async Task CreateAsync(Property property)
    {
        var result = new ValidationResult(PropertyEntityValidator.Validate(property));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(Property).ToLower()} (ID: {property.Id}).", result.Errors);

        _repositories.Properties.Create(property);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task UpdateAsync(Property property)
    {
        property.LastUpdatedAt = DateTimeOffset.UtcNow;

        var result = new ValidationResult(PropertyEntityValidator.Validate(property));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(Property).ToLower()} (ID: {property.Id}).", result.Errors);

        _repositories.Properties.Update(property);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task DeleteAsync(Property property)
    {
        _repositories.Properties.Delete(property);
        await _repositories.SaveOrThrowAsync();
    }
}