using System.Net.Mail;
using InvenireServer.Application.Dtos.Employees.Email;
using InvenireServer.Application.Interfaces.Email.Builders;

namespace InvenireServer.Infrastructure.Email.Builders;

/// <summary>
/// Default implementation of <see cref="IEmployeeEmailBuilder"/>.
/// </summary>
public class EmployeeEmailBuilder : BaseEmailBuilder, IEmployeeEmailBuilder
{
    public EmployeeEmailBuilder(string source) : base(source)
    {
    }

    /// <summary>
    /// Builds a recovery email for an employee.
    /// </summary>
    /// <param name="dto">Data used to populate the recovery email template.</param>
    /// <returns>Composed email message.</returns>
    public MailMessage BuildRecoveryEmail(EmployeeRecoveryEmailDto dto)
    {
        var message = new MailMessage(SourceAddress, dto.EmployeeAddress)
        {
            Body = ParseHtmlTemplate(Path.Combine(AppContext.BaseDirectory, "assets", "templates", "employee_recovery_email_template.html"), dto),
            Subject = "Obnovení účtu.",
            IsBodyHtml = true,
        };

        return message;
    }

    /// <summary>
    /// Builds a verification email for an employee.
    /// </summary>
    /// <param name="dto">Data used to populate the verification email template.</param>
    /// <returns>Composed email message.</returns>
    public MailMessage BuildVerificationEmail(EmployeeVerificationEmailDto dto)
    {
        var message = new MailMessage(SourceAddress, dto.EmployeeAddress)
        {
            Body = ParseHtmlTemplate(Path.Combine(AppContext.BaseDirectory, "assets", "templates", "employee_verification_email_template.html"), dto),
            Subject = "Ověřte svojí emailovou adresu.",
            IsBodyHtml = true,
        };

        return message;
    }
}
