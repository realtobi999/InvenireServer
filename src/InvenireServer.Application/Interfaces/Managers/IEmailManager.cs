using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Email.Builders;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IEmailManager
{
    IEmailSender Sender { get; }

    IEmailBuilderGroup Builders { get; }
}

public interface IEmailBuilderGroup
{
    public IAdminEmailBuilder Admin { get; }

    public IEmployeeEmailBuilder Employee { get; }
}

public class EmailBuilderGroup : IEmailBuilderGroup
{
    public required IAdminEmailBuilder Admin { get; set; }

    public required IEmployeeEmailBuilder Employee { get; set; }
}