using System.Net.Mail;

namespace InvenireServer.Domain.Core.Interfaces.Email;

public interface IEmailSender
{
    string SourceAddress { get; }
    Task SendEmailAsync(MailMessage message);
}
