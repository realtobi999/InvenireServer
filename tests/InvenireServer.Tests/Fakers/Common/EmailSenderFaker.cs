using System.Net.Mail;
using InvenireServer.Application.Interfaces.Email;

namespace InvenireServer.Tests.Fakers.Common;

/// <summary>
/// Fake email sender that captures messages for tests.
/// </summary>
public class EmailSenderFaker : IEmailSender
{
    public List<MailMessage> CapturedMessages { get; } = [];
    public string SourceAddress => "invalid_testing_email@test.com";

    /// <summary>
    /// Captures the email message for inspection.
    /// </summary>
    /// <param name="message">Email message to capture.</param>
    /// <returns>Awaitable task representing the send operation.</returns>
    public Task SendEmailAsync(MailMessage message)
    {
        CapturedMessages.Add(message);
        return Task.CompletedTask;
    }
}
