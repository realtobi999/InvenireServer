using System.Net.Mail;
using InvenireServer.Application.Dtos.Employees.Email;
using InvenireServer.Application.Interfaces.Email.Builders;

namespace InvenireServer.Infrastructure.Email.Builders;

public class EmployeeEmailBuilder : BaseEmailBuilder, IEmployeeEmailBuilder
{
    public EmployeeEmailBuilder(string source) : base(source)
    {
    }

    public MailMessage BuildVerificationEmail(EmployeeVerificationEmailDto dto)
    {
        var message = new MailMessage(SourceAddress, dto.EmployeeAddress)
        {
            IsBodyHtml = true
        };

        message.Subject = "Please verify your email to complete your registration";
        message.Body = ParseHtmlTemplate(Path.Combine(AppContext.BaseDirectory, "assets", "templates", "employee_verification_email_template.html"), dto);

        return message;
    }
}