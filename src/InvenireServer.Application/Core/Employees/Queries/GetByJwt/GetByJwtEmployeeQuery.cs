using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Queries.GetByJwt;

public record GetByJwtEmployeeQuery : IRequest<GetByJwtEmployeeQueryResponse>
{
    public required Jwt Jwt { get; set; }
}
