using System.Net.Mail;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Email.Builders;

namespace InvenireServer.Infrastructure.Email.Builders;

public class AdminEmailBuilder : BaseEmailBuilder, IAdminEmailBuilder
{
    public AdminEmailBuilder(string source) : base(source)
    {
    }

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