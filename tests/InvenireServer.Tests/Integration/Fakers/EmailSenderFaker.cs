using System.Net.Mail;
using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Tests.Integration.Fakers;

public class EmailSenderFaker : IEmailSender
{
    public Task SendEmailAsync(MailMessage message)
    {
        return Task.CompletedTask;
    }
}
