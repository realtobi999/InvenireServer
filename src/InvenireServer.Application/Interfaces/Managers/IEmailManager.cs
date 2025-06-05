using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Email.Builders;

namespace InvenireServer.Application.Interfaces.Managers;

/// <summary>
/// Manages email sending operations by coordinating the email sender and employee email builder components.
/// </summary>
public interface IEmailManager
{
    /// <summary>
    /// Gets the email sender responsible for sending email messages.
    /// </summary>
    IEmailSender Sender { get; }

    IEmailBuilderGroup Builders { get; }
}

public interface IEmailBuilderGroup
{
    IAdminEmailBuilder Admin { get; }
    IEmployeeEmailBuilder Employee { get; }
}

public class EmailBuilderGroup : IEmailBuilderGroup
{
    public required IAdminEmailBuilder Admin { get; set; }
    public required IEmployeeEmailBuilder Employee { get; set; }
}