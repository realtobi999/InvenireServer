using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Queries.GetByJwt;

/// <summary>
/// Represents a query to get an admin using a JWT.
/// </summary>
public record GetByJwtAdminQuery : IRequest<AdminDto>
{
    public required Jwt Jwt { get; init; }
}
