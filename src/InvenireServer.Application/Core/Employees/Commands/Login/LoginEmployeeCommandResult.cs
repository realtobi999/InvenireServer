using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Commands.Login;

public class LoginEmployeeCommandResult
{
    public required Jwt Token { get; init; }
    public required string TokenString { get; init; }
}


