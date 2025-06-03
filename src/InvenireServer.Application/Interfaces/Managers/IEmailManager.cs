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

    /// <summary>
    /// Gets the builder used to create employee-specific email messages.
    /// </summary>
    IEmployeeEmailBuilder EmployeeBuilder { get; }
}
