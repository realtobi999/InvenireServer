using System.Net.Mail;
using InvenireServer.Domain.Core.Dtos.Employees.Emails;
using InvenireServer.Domain.Core.Interfaces.Email.Builders;

namespace InvenireServer.Infrastructure.Email.Builders;

/// <summary>
/// Builds email messages for employee-related actions.
/// </summary>
public class EmployeeEmailBuilder : BaseEmailBuilder, IEmployeeEmailBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeEmailBuilder"/> class.
    /// </summary>
    /// <param name="source">The email address used as the sender of outgoing messages.</param>
    public EmployeeEmailBuilder(string source) : base(source)
    {
    }

    /// <summary>
    /// Builds an email message prompting the employee to verify their email address.
    /// </summary>
    /// <param name="dto">The data transfer object containing the employee's name, email address, and verification link.</param>
    /// <returns>An HTML-formatted <see cref="MailMessage"/> object ready to be sent.</returns>
    public MailMessage BuildVerificationEmail(EmployeeVerificationEmailDto dto)
    {
        var message = new MailMessage(this.SourceAddress, dto.EmployeeAddress)
        {
            IsBodyHtml = true
        };

        message.Subject = "Please verify your email to complete your registration";
        message.Body = this.ParseHtmlTemplate(Path.Combine(AppContext.BaseDirectory, "assets", "templates", "employee_verification_email_template.html"), dto);

        return message;
    }
}
