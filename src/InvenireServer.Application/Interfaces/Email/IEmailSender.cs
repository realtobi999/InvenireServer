using System.Net.Mail;

namespace InvenireServer.Application.Interfaces.Email;

/// <summary>
/// Defines a contract for sending email messages.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Gets the source email address used as the sender of outgoing emails.
    /// </summary>
    string SourceAddress { get; }

    /// <summary>
    /// Sends the specified email message asynchronously.
    /// </summary>
    /// <param name="message">The email message to send.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task SendEmailAsync(MailMessage message);
}