using System.Net.Mail;

namespace InvenireServer.Application.Interfaces.Email;

public interface IEmailSender
{
    string SourceAddress { get; }

    Task SendEmailAsync(MailMessage message);
}