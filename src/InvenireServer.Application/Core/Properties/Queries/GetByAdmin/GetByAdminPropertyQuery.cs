using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Core.Properties.Queries.GetByAdmin;

/// <summary>
/// Represents a query to get a property for an admin.
/// </summary>
public record GetByAdminPropertyQuery : IRequest<PropertyDto>
{
    public required Jwt Jwt { get; init; }
}
