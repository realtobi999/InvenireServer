using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;

/// <summary>
/// Represents a query to index property scans for an admin.
/// </summary>
public record IndexByAdminPropertyScanQuery : IRequest<IndexByAdminPropertyScanQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required IndexByAdminPropertyScanQueryParameters Parameters { get; init; }
}
