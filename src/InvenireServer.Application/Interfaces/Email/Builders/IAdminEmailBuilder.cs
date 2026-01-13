using System.Net.Mail;
using InvenireServer.Application.Dtos.Admins.Email;

namespace InvenireServer.Application.Interfaces.Email.Builders;

/// <summary>
/// Defines a builder for admin-related emails.
/// </summary>
public interface IAdminEmailBuilder
{
    /// <summary>
    /// Builds a recovery email for an admin.
    /// </summary>
    /// <param name="dto">Data used to populate the recovery email template.</param>
    /// <returns>Composed email message.</returns>
    MailMessage BuildRecoveryEmail(AdminRecoveryEmailDto dto);

    /// <summary>
    /// Builds a verification email for an admin.
    /// </summary>
    /// <param name="dto">Data used to populate the verification email template.</param>
    /// <returns>Composed email message.</returns>
    MailMessage BuildVerificationEmail(AdminVerificationEmailDto dto);
}
