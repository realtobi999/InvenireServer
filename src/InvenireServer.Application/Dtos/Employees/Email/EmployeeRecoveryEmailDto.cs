namespace InvenireServer.Application.Dtos.Employees.Email;

/// <summary>
/// Represents the information required to construct an admin recovery email.
/// </summary>
public record EmployeeRecoveryEmailDto
{
    public required string EmployeeAddress { get; init; }
    public required string RecoveryLink { get; init; }
}
