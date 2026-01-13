using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Email.Builders;

namespace InvenireServer.Application.Interfaces.Managers;

/// <summary>
/// Defines a manager for email building and sending.
/// </summary>
public interface IEmailManager
{
    /// <summary>
    /// Email sender used to send messages.
    /// </summary>
    IEmailSender Sender { get; }

    /// <summary>
    /// Email builder group used to compose messages.
    /// </summary>
    IEmailBuilderGroup Builders { get; }
}

/// <summary>
/// Defines a group of email builders by recipient type.
/// </summary>
public interface IEmailBuilderGroup
{
    /// <summary>
    /// Admin email builder.
    /// </summary>
    public IAdminEmailBuilder Admin { get; }

    /// <summary>
    /// Employee email builder.
    /// </summary>
    public IEmployeeEmailBuilder Employee { get; }
}

/// <summary>
/// Default implementation of <see cref="IEmailBuilderGroup"/>.
/// </summary>
public class EmailBuilderGroup : IEmailBuilderGroup
{
    /// <summary>
    /// Admin email builder.
    /// </summary>
    public required IAdminEmailBuilder Admin { get; init; }

    /// <summary>
    /// Employee email builder.
    /// </summary>
    public required IEmployeeEmailBuilder Employee { get; init; }
}
