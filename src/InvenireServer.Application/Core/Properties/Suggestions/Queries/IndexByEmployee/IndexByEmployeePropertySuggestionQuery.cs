using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByEmployee;

/// <summary>
/// Represents a query to index property suggestions for an employee.
/// </summary>
public record IndexByEmployeePropertySuggestionQuery : IRequest<IndexByEmployeePropertySuggestionQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required IndexByEmployeePropertySuggestionQueryParameters Parameters { get; init; }
}
