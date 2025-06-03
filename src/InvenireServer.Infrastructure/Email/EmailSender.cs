using System.Net.Mail;
using InvenireServer.Application.Interfaces.Email;

namespace InvenireServer.Infrastructure.Email;

/// <summary>
/// Handles the sending of emails using an SMTP client.
/// </summary>
public class EmailSender : IEmailSender
{
    private readonly SmtpClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSender"/> class using the specified SMTP client and source email address.
    /// </summary>
    /// <param name="client">The SMTP client used to send emails.</param>
    /// <param name="address">The email address used as the sender.</param>
    public EmailSender(SmtpClient client, string address)
    {
        _client = client;
        SourceAddress = address;
    }

    /// <inheritdoc/>
    public string SourceAddress { get; }

    /// <inheritdoc/>
    public async Task SendEmailAsync(MailMessage message)
    {
        await _client.SendMailAsync(message);
    }
}