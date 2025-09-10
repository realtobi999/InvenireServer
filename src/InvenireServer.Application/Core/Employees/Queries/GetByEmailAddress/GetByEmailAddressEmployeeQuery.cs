using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Queries.GetByEmailAddress;

public class GetByEmailAddressEmployeeQuery : IRequest<EmployeeDto>
{
    public required Jwt Jwt { get; init; }
    public required string EmployeeEmailAddress { get; init; }
}
