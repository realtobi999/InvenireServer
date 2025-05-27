namespace InvenireServer.Domain.Core.Dtos.Employees.Emails;

public record EmployeeVerificationEmailDto
{
    public required string EmployeeAddress { get; set; }
    public required string EmployeeName { get; set; }
    public required string VerificationLink { get; set; }
}
