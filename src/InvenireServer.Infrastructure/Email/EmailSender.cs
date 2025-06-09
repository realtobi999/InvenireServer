using System.Net.Mail;
using InvenireServer.Application.Interfaces.Email;

namespace InvenireServer.Infrastructure.Email;

public class EmailSender : IEmailSender
{
    private readonly SmtpClient _client;

    public EmailSender(SmtpClient client, string address)
    {
        _client = client;
        SourceAddress = address;
    }

    public string SourceAddress { get; }

    public async Task SendEmailAsync(MailMessage message)
    {
        await _client.SendMailAsync(message);
    }
}