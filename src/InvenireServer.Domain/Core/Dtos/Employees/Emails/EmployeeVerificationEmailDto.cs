namespace InvenireServer.Domain.Core.Dtos.Employees.Emails;

public record EmployeeVerificationEmailDto
{
    public required string EmployeeAddress { get; init; }
    public required string EmployeeName { get; init; }
    public required string VerificationLink { get; init; }
}
