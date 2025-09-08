using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByAdmin;

public record IndexByAdminPropertySuggestionQuery : IRequest<IndexByAdminPropertySuggestionQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required PaginationOptions Pagination { get; init; }
}
