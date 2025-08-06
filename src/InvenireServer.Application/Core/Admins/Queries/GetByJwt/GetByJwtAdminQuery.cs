using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Queries.GetByJwt;

public record GetByJwtAdminQuery : IRequest<AdminDto>
{
    public required Jwt Jwt { get; set; }
}
