using System.Net.Mail;

namespace InvenireServer.Domain.Core.Interfaces.Common;

public interface IEmailSender
{
    Task SendEmailAsync(MailMessage message);
}
