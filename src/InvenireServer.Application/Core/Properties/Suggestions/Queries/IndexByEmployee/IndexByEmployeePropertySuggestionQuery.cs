using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByEmployee;

public record IndexByEmployeePropertySuggestionQuery : IRequest<IndexByEmployeePropertySuggestionQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required IndexByEmployeePropertySuggestionQueryParameters Parameters { get; init; }
}
