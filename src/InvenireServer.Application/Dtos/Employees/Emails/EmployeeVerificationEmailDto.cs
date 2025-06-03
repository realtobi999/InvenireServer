namespace InvenireServer.Application.Dtos.Employees.Emails;

/// <summary>
///  Represents a verification email to an employee.
/// </summary>
public record EmployeeVerificationEmailDto
{
    /// <summary>
    /// Email address of the employee receiving the verification link.
    /// </summary>
    public required string EmployeeAddress { get; init; }

    /// <summary>
    /// Full name of the employee for personalization in the email content.
    /// </summary>
    public required string EmployeeName { get; init; }

    /// <summary>
    /// URL that the employee must visit to complete the verification process.
    /// </summary>
    public required string VerificationLink { get; init; }
}