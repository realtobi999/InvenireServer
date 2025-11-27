namespace InvenireServer.Application.Dtos.Employees.Email;

public record EmployeeRecoveryEmailDto
{
    public required string EmployeeAddress { get; init; }
    public required string RecoveryLink { get; init; }
}
