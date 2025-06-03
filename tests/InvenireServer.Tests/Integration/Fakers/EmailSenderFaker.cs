using System.Net.Mail;
using InvenireServer.Application.Interfaces.Email;

namespace InvenireServer.Tests.Integration.Fakers;

/// <summary>
/// A test implementation of <see cref="IEmailSender"/> that captures sent emails in-memory instead of actually sending them. 
/// </summary>
public class EmailSenderFaker : IEmailSender
{
    /// <summary>
    /// Gets the fixed source email address used as the sender in tests.
    /// </summary>
    public string SourceAddress => "invalid_testing_email@test.com";

    /// <summary>
    /// Gets the list of all email messages that have been "sent" and captured during tests.
    /// </summary>
    public List<MailMessage> CapturedMessages { get; } = [];

    /// <summary>
    /// Simulates sending an email by adding the <paramref name="message"/> to the captured messages list.
    /// </summary>
    /// <param name="message">The email message to capture.</param>
    /// <returns>A completed task to satisfy the asynchronous method signature.</returns>
    public Task SendEmailAsync(MailMessage message)
    {
        CapturedMessages.Add(message);
        return Task.CompletedTask;
    }
}