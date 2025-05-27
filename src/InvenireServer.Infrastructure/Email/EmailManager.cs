using InvenireServer.Domain.Core.Interfaces.Email;
using InvenireServer.Domain.Core.Interfaces.Email.Builders;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Infrastructure.Email.Builders;

namespace InvenireServer.Infrastructure.Email;

public class EmailManager : IEmailManager
{
    public EmailManager(IEmailSender sender)
    {
        Sender = sender;
        EmployeeBuilder = new EmployeeEmailBuilder(sender.SourceAddress);
    }

    public IEmailSender Sender { get; }
    public IEmployeeEmailBuilder EmployeeBuilder { get; }
}
