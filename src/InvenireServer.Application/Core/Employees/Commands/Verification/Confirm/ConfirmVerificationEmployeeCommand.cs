using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Commands.Verification.Confirm;

public class ConfirmVerificationEmployeeCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}