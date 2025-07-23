using InvenireServer.Application.Dtos.Admins;

namespace InvenireServer.Application.Core.Admins.Queries.GetByJwt;

public class GetByJwtAdminQueryResponse
{
    public required AdminDto AdminDto { get; set; }
}
