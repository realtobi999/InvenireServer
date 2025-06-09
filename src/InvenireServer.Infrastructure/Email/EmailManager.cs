using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Infrastructure.Email.Builders;

namespace InvenireServer.Infrastructure.Email;

public class EmailManager : IEmailManager
{
    private readonly Lazy<EmailBuilderGroup> _builders;
    private readonly Lazy<IEmailSender> _sender;

    public EmailManager(IEmailSender sender)
    {
        _sender = new Lazy<IEmailSender>(sender);
        _builders = new Lazy<EmailBuilderGroup>(new EmailBuilderGroup
        {
            Admin = new AdminEmailBuilder(sender.SourceAddress),
            Employee = new EmployeeEmailBuilder(sender.SourceAddress)
        });
    }

    public EmailBuilderGroup Builders => _builders.Value;

    public IEmailSender Sender => _sender.Value;
}