using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Employees.Commands.Register;

public record RegisterEmployeeCommandResult
{
    public required Employee Employee { get; init; }
    public required string Token { get; init; }
}
