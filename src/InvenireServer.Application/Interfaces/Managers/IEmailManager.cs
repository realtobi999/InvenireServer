using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Email.Builders;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IEmailManager
{
    IEmailSender Sender { get; }

    EmailBuilderGroup Builders { get; }
}

public class EmailBuilderGroup
{
    public required IAdminEmailBuilder Admin { get; set; }

    public required IEmployeeEmailBuilder Employee { get; set; }
}