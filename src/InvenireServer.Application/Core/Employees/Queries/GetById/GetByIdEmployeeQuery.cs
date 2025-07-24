using InvenireServer.Application.Dtos.Employees;

namespace InvenireServer.Application.Core.Employees.Queries.GetById;

public record GetByIdEmployeeQuery : IRequest<EmployeeDto>
{
    public required Guid EmployeeId { get; set; }
}
