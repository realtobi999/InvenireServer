using System.Net.Mail;

namespace InvenireServer.Application.Interfaces.Email;

/// <summary>
/// Defines an email sender.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Source email address for outgoing messages.
    /// </summary>
    string SourceAddress { get; }

    /// <summary>
    /// Sends the provided email message.
    /// </summary>
    /// <param name="message">Email message to send.</param>
    /// <returns>Awaitable task representing the send operation.</returns>
    Task SendEmailAsync(MailMessage message);
}
