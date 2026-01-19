using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Commands.Verification.Confirm;

/// <summary>
/// Represents a request to confirm a verification for an employee.
/// </summary>
public record ConfirmVerificationEmployeeCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}