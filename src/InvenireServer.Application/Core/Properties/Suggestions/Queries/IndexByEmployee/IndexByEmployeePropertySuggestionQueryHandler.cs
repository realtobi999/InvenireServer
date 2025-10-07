using System.Text.Json;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByEmployee;

public class IndexByEmployeePropertySuggestionQueryHandler : IRequestHandler<IndexByEmployeePropertySuggestionQuery, IndexByEmployeePropertySuggestionQueryResponse>
{
    private readonly IRepositoryManager _repositories;

    public IndexByEmployeePropertySuggestionQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<IndexByEmployeePropertySuggestionQueryResponse> Handle(IndexByEmployeePropertySuggestionQuery request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(employee) ?? throw new BadRequest400Exception("The employee isn't part of any organization.");
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
                    s => s.PropertyId == property.Id && employee.Id == employee.Id,

                    // Search Filter.
                    request.Parameters.SearchQuery is not null ? _repositories.Properties.Suggestions.BuildSearchExpression(request.Parameters.SearchQuery) : null,

                    // Additional Filters.
                    request.Parameters.Status is not null ? s => s.Status == request.Parameters.Status : null
                ]
            },
            Pagination = new QueryPaginationOptions(request.Parameters.Limit, request.Parameters.Offset)
        };
        var suggestions = await _repositories.Properties.Suggestions.IndexAsync(query);

        // Load the employee and deserialize payload.
        var dto = EmployeeDto.BaseSelector.Compile()(employee);
        foreach (var suggestion in suggestions)
        {
            suggestion.Employee = dto;
            suggestion.Payload = JsonSerializer.Deserialize<PropertySuggestionPayload>(suggestion.PayloadString!);
        }

        return new IndexByEmployeePropertySuggestionQueryResponse
        {
            Data = [.. suggestions],
            Limit = request.Parameters.Limit,
            Offset = request.Parameters.Offset,
            TotalCount = await _repositories.Properties.Suggestions.CountAsync(query.Filtering.Filters!)
        };
    }
}
