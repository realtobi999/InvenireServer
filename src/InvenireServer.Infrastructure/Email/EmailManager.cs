using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Email.Builders;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
using InvenireServer.Infrastructure.Email.Builders;

namespace InvenireServer.Infrastructure.Email;

/// <summary>
/// Provides access to email-related components, including message builders and the sender.
/// </summary>
public class EmailManager : IEmailManager
{
    private readonly Lazy<IEmailSender> _sender;
    private readonly Lazy<IEmailBuilderGroup> _builders;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailManager"/> class using the specified sender.
    /// </summary>
    /// <param name="sender">The email sender responsible for dispatching messages.</param>
    public EmailManager(IEmailSender sender)
    {
        _sender = new Lazy<IEmailSender>(sender);
        _builders = new Lazy<IEmailBuilderGroup>(new EmailBuilderGroup
        {
            Admin = new AdminEmailBuilder(sender.SourceAddress),
            Employee = new EmployeeEmailBuilder(sender.SourceAddress),
        });
    }

    /// <inheritdoc/>
    public IEmailSender Sender => _sender.Value;

    /// <inheritdoc/>
    public IEmailBuilderGroup Builders => _builders.Value;
}