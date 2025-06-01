using InvenireServer.Domain.Core.Interfaces.Email;
using InvenireServer.Domain.Core.Interfaces.Email.Builders;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Infrastructure.Email.Builders;

namespace InvenireServer.Infrastructure.Email;

/// <summary>
/// Provides access to email-related components, including message builders and the sender.
/// </summary>
public class EmailManager : IEmailManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailManager"/> class using the specified sender.
    /// </summary>
    /// <param name="sender">The email sender responsible for dispatching messages.</param>
    public EmailManager(IEmailSender sender)
    {
        Sender = sender;
        EmployeeBuilder = new EmployeeEmailBuilder(sender.SourceAddress);
    }

    /// <inheritdoc/>
    public IEmailSender Sender { get; }

    /// <inheritdoc/>
    public IEmployeeEmailBuilder EmployeeBuilder { get; }
}
