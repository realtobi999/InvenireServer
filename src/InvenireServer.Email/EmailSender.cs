using System.Net.Mail;
using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Email;

public class EmailSender : IEmailSender
{
    private readonly SmtpClient _client;

    public EmailSender(SmtpClient client)
    {
        _client = client;
    }

    public async Task SendEmailAsync(MailMessage message)
    {
        await _client.SendMailAsync(message);
    }
}
