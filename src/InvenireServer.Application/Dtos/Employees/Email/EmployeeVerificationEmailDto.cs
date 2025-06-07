namespace InvenireServer.Application.Dtos.Employees.Email;

public record EmployeeVerificationEmailDto
{
    public required string EmployeeAddress { get; init; }

    public required string EmployeeName { get; init; }

    public required string VerificationLink { get; init; }
}