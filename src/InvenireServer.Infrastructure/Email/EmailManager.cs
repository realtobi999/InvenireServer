using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Infrastructure.Email.Builders;

namespace InvenireServer.Infrastructure.Email;

/// <summary>
/// Default implementation of <see cref="IEmailManager"/>.
/// </summary>
public class EmailManager : IEmailManager
{
    private readonly Lazy<IEmailBuilderGroup> _builders;
    private readonly Lazy<IEmailSender> _sender;

    public EmailManager(IEmailSender sender)
    {
        _sender = new Lazy<IEmailSender>(sender);
        _builders = new Lazy<IEmailBuilderGroup>(new EmailBuilderGroup
        {
            Admin = new AdminEmailBuilder(sender.SourceAddress),
            Employee = new EmployeeEmailBuilder(sender.SourceAddress)
        });
    }

    public IEmailBuilderGroup Builders => _builders.Value;

    public IEmailSender Sender => _sender.Value;
}
