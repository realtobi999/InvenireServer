using System.Net.Mail;
using InvenireServer.Application.Dtos.Employees.Email;

namespace InvenireServer.Application.Interfaces.Email.Builders;

/// <summary>
/// Defines a builder for employee-related emails.
/// </summary>
public interface IEmployeeEmailBuilder
{
    /// <summary>
    /// Builds a recovery email for an employee.
    /// </summary>
    /// <param name="dto">Data used to populate the recovery email template.</param>
    /// <returns>Composed email message.</returns>
    MailMessage BuildRecoveryEmail(EmployeeRecoveryEmailDto dto);

    /// <summary>
    /// Builds a verification email for an employee.
    /// </summary>
    /// <param name="dto">Data used to populate the verification email template.</param>
    /// <returns>Composed email message.</returns>
    MailMessage BuildVerificationEmail(EmployeeVerificationEmailDto dto);
}
