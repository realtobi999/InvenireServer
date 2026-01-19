using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByAdmin;

/// <summary>
/// Represents a query to index property items for an admin.
/// </summary>
public record IndexByAdminPropertyItemQuery : IRequest<IndexByAdminPropertyItemQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required IndexByAdminPropertyItemQueryParameters Parameters { get; init; }
}
