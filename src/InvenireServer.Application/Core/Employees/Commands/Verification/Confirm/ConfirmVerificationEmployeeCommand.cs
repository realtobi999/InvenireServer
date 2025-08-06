using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Commands.Verification.Confirm;

public record ConfirmVerificationEmployeeCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}