using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Commands.Delete;

public record DeleteEmployeeCommand : IRequest
{
    public required Jwt Jwt { get; set; }
}
