using InvenireServer.Domain.Core.Interfaces.Email;
using InvenireServer.Domain.Core.Interfaces.Email.Builders;

namespace InvenireServer.Domain.Core.Interfaces.Managers;

public interface IEmailManager
{
    IEmailSender Sender { get; }
    IEmployeeEmailBuilder EmployeeBuilder { get; }
}
