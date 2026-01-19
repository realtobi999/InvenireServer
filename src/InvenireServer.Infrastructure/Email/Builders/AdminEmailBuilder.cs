using System.Net.Mail;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Email.Builders;

namespace InvenireServer.Infrastructure.Email.Builders;

/// <summary>
/// Default implementation of <see cref="IAdminEmailBuilder"/>.
/// </summary>
public class AdminEmailBuilder : BaseEmailBuilder, IAdminEmailBuilder
{
    public AdminEmailBuilder(string source) : base(source)
    {
    }

    /// <summary>
    /// Builds a recovery email for an admin.
    /// </summary>
    /// <param name="dto">Data used to populate the recovery email template.</param>
    /// <returns>Composed email message.</returns>
    public MailMessage BuildRecoveryEmail(AdminRecoveryEmailDto dto)
    {
        var message = new MailMessage(SourceAddress, dto.AdminAddress)
        {
            Body = ParseHtmlTemplate(Path.Combine(AppContext.BaseDirectory, "assets", "templates", "admin_recovery_email_template.html"), dto),
            Subject = "Obnovení účtu.",
            IsBodyHtml = true,
        };

        return message;
    }

    /// <summary>
    /// Builds a verification email for an admin.
    /// </summary>
    /// <param name="dto">Data used to populate the verification email template.</param>
    /// <returns>Composed email message.</returns>
    public MailMessage BuildVerificationEmail(AdminVerificationEmailDto dto)
    {
        var message = new MailMessage(SourceAddress, dto.AdminAddress)
        {
            Body = ParseHtmlTemplate(Path.Combine(AppContext.BaseDirectory, "assets", "templates", "admin_verification_email_template.html"), dto),
            Subject = "Ověřte svojí emailovou adresu.",
            IsBodyHtml = true,
        };

        return message;
    }
}
