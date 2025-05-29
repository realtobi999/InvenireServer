using System.Net.Mail;
using InvenireServer.Domain.Core.Interfaces.Email;

namespace InvenireServer.Tests.Integration.Fakers;

public class EmailSenderFaker : IEmailSender
{
    public string SourceAddress => "invalid_testing_email@test.com";
    public List<MailMessage> CapturedMessages { get; } = [];

    public Task SendEmailAsync(MailMessage message)
    {
        CapturedMessages.Add(message);
        return Task.CompletedTask;
    }
}
