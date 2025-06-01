using System.Net.Mail;
using InvenireServer.Domain.Core.Dtos.Employees.Emails;

namespace InvenireServer.Domain.Core.Interfaces.Email.Builders;

/// <summary>
/// Defines a contract for building email messages related to employee verification processes.
/// </summary>
public interface IEmployeeEmailBuilder
{
    /// <summary>
    /// Creates a verification email message based on the provided data transfer object.
    /// </summary>
    /// <param name="dto">The data containing employee email, name, and verification link.</param>
    /// <returns>A <see cref="MailMessage"/> configured for employee verification.</returns>
    MailMessage BuildVerificationEmail(EmployeeVerificationEmailDto dto);
}
