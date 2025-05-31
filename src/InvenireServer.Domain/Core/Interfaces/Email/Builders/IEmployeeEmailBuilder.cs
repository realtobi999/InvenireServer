using System.Net.Mail;
using InvenireServer.Domain.Core.Dtos.Employees.Emails;

namespace InvenireServer.Domain.Core.Interfaces.Email.Builders;

public interface IEmployeeEmailBuilder
{
    MailMessage BuildVerificationEmail(EmployeeVerificationEmailDto dto);
}
