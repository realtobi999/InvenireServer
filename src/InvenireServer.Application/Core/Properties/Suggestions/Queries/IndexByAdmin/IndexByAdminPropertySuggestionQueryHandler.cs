using System.Text.Json;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
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
            Ordering = new QueryOrderingOptions<PropertySuggestion>(request.Parameters.Order, request.Parameters.Desc),
            Selector = PropertySuggestionDto.IndexByAdminSelector,
            Filtering = new QueryFilteringOptions<PropertySuggestion>
            {
                Filters =
                [
                    // Core Filter.
                    s => s.PropertyId == property.Id,

                    // Search Filter.
                    request.Parameters.SearchQuery is not null ? _repositories.Properties.Suggestions.BuildSearchExpression(request.Parameters.SearchQuery) : null,

                    // Additional Filters.
                    request.Parameters.Status is not null ? s => s.Status == request.Parameters.Status : null
                ]
            },
            Pagination = new QueryPaginationOptions(request.Parameters.Limit, request.Parameters.Offset)
        };

        var suggestions = await _repositories.Properties.Suggestions.IndexAsync(query);

        // Load all the employees and deserialize payload.
        foreach (var suggestion in suggestions)
        {
            suggestion.Employee = await _repositories.Employees.GetAsync(new QueryOptions<Employee, EmployeeDto>
            {
                Selector = EmployeeDto.BaseSelector,
                Filtering = new QueryFilteringOptions<Employee>
                {
                    Filters = [e => e.Id == suggestion.EmployeeId]
                },
            });
            suggestion.Payload = JsonSerializer.Deserialize<PropertySuggestionPayload>(suggestion.PayloadString!);
        }

        return new IndexByAdminPropertySuggestionQueryResponse
        {
            Data = [.. suggestions],
            Limit = request.Parameters.Limit,
            Offset = request.Parameters.Offset,
            TotalCount = await _repositories.Properties.Suggestions.CountAsync(query.Filtering.Filters!)
        };
    }
}
