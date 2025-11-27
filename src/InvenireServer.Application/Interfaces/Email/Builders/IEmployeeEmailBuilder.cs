using System.Net.Mail;
using InvenireServer.Application.Dtos.Employees.Email;

namespace InvenireServer.Application.Interfaces.Email.Builders;

public interface IEmployeeEmailBuilder
{
    MailMessage BuildRecoveryEmail(EmployeeRecoveryEmailDto dto);
    MailMessage BuildVerificationEmail(EmployeeVerificationEmailDto dto);
}