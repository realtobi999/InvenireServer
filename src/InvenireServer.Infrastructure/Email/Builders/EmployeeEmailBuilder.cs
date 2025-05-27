using System.Net.Mail;
using InvenireServer.Domain.Core.Dtos.Employees.Emails;
using InvenireServer.Domain.Core.Interfaces.Email.Builders;

namespace InvenireServer.Infrastructure.Email.Builders;

public class EmployeeEmailBuilder : BaseEmailBuilder, IEmployeeEmailBuilder
{
    public EmployeeEmailBuilder(string source) : base(source)
    {
    }

    public MailMessage BuildVerificationEmail(EmployeeVerificationEmailDto dto)
    {
        var message = new MailMessage(this.SourceAddress, dto.EmployeeAddress)
        {
            IsBodyHtml = true
        };

        message.Subject = "Please verify your email to complete your registration";
        message.Body = this.ParseHtmlTemplate($"{AppContext.BaseDirectory}\\Email\\Builders\\Templates\\employee_verification_email_template.html", dto);

        return message;
    }

    public MailMessage BuildVerificationSuccessEmail(EmployeeVerificationSuccessEmailDto dto)
    {
        throw new NotImplementedException();
    }
}
