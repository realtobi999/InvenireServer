using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Queries.GetByJwt;

public class GetByJwtAdminQuery : IRequest<GetByJwtAdminQueryResponse>
{
    public required Jwt Jwt { get; set; }
}
