using System.Net.Mail;
using InvenireServer.Application.Dtos.Admins.Email;

namespace InvenireServer.Application.Interfaces.Email.Builders;

public interface IAdminEmailBuilder
{
    MailMessage BuildRecoveryEmail(AdminRecoveryEmailDto dto);
    MailMessage BuildVerificationEmail(AdminVerificationEmailDto dto);
}