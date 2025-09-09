using System.Text.Json;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByAdmin;

public class IndexByAdminPropertySuggestionQueryHandler : IRequestHandler<IndexByAdminPropertySuggestionQuery, IndexByAdminPropertySuggestionQueryResponse>
{
    private readonly IRepositoryManager _repositories;

    public IndexByAdminPropertySuggestionQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<IndexByAdminPropertySuggestionQueryResponse> Handle(IndexByAdminPropertySuggestionQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var query = new QueryOptions<PropertySuggestion, PropertySuggestionDto>
        {
            Selector = PropertySuggestionDto.IndexByAdminSelector,
            Filtering = new QueryFilteringOptions<PropertySuggestion>
            {
                Filters =
                [
                    s => s.PropertyId == property.Id
                ]
            },
            Pagination = request.Pagination
        };

        var suggestions = await _repositories.Properties.Suggestions.IndexAsync(query);
        suggestions = suggestions.Select(suggestion =>
        {
            return suggestion with
            {
                Payload = JsonSerializer.Deserialize<PropertySuggestionPayload>(suggestion.PayloadString!)
            };
        });

        return new IndexByAdminPropertySuggestionQueryResponse
        {
            Data = [.. suggestions],
            Limit = request.Pagination.Limit,
            Offset = request.Pagination.Offset,
            TotalCount = await _repositories.Properties.Suggestions.CountAsync(query.Filtering.Filters!)
        };
    }
}
