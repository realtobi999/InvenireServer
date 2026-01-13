namespace InvenireServer.Application.Dtos.Employees.Email;

/// <summary>
/// Represents the information required to construct an employee verification email.
/// </summary>
public record EmployeeVerificationEmailDto
{
    public required string EmployeeAddress { get; init; }
    public required string EmployeeFirstName { get; init; }
    public required string VerificationLink { get; init; }
}