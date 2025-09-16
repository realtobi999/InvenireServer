using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByAdmin;

public record IndexByAdminPropertySuggestionQuery : IRequest<IndexByAdminPropertySuggestionQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required IndexByAdminPropertySuggestionQueryParameters Parameters { get; init; }
}
