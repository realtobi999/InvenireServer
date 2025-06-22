using System.Net.Mail;
using InvenireServer.Application.Interfaces.Email;

namespace InvenireServer.Tests.Integration.Fakers.Common;

public class EmailSenderFaker : IEmailSender
{
    public List<MailMessage> CapturedMessages { get; } = [];
    public string SourceAddress => "invalid_testing_email@test.com";

    public Task SendEmailAsync(MailMessage message)
    {
        CapturedMessages.Add(message);
        return Task.CompletedTask;
    }
}