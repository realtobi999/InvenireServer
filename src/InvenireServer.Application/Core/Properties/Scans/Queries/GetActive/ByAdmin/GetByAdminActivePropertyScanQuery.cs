using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.GetActive.ByAdmin;

/// <summary>
/// Represents a query to get an active property scan for an admin.
/// </summary>
public record GetByAdminActivePropertyScanQuery : IRequest<PropertyScanDto>
{
    public required Jwt Jwt { get; init; }
}
