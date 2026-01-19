using System.Net.Mail;
using InvenireServer.Application.Interfaces.Email;

namespace InvenireServer.Infrastructure.Email;

/// <summary>
/// Default implementation of <see cref="IEmailSender"/>.
/// </summary>
public class EmailSender : IEmailSender
{
    private readonly SmtpClient _client;

    public EmailSender(SmtpClient client, string address)
    {
        _client = client;
        SourceAddress = address;
    }

    public string SourceAddress { get; }

    /// <summary>
    /// Sends the provided email message.
    /// </summary>
    /// <param name="message">Email message to send.</param>
    /// <returns>Awaitable task representing the send operation.</returns>
    public async Task SendEmailAsync(MailMessage message)
    {
        await _client.SendMailAsync(message);
    }
}
