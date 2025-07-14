using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Interfaces.Services.Properties;
using System.Linq.Expressions;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Services.Properties;

public class PropertySuggestionService : IPropertySuggestionService
{
    private readonly IRepositoryManager _repositories;

    public PropertySuggestionService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<PropertySuggestion> GetAsync(Expression<Func<PropertySuggestion, bool>> predicate)
    {
        var suggestion = await _repositories.Properties.Suggestions.GetAsync(predicate);

        if (suggestion is null) throw new NotFound404Exception($"The requested {nameof(PropertySuggestion).ToLower()} was not found in the system");

        return suggestion;
    }

    public async Task CreateAsync(PropertySuggestion suggestion)
    {
        _repositories.Properties.Suggestions.Create(suggestion);
        await _repositories.SaveOrThrowAsync();
    }


    public async Task UpdateAsync(PropertySuggestion suggestion)
    {
        _repositories.Properties.Suggestions.Update(suggestion);
        await _repositories.SaveOrThrowAsync();
    }
}
