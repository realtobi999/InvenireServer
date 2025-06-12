using System.Net.Mail;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Email.Builders;

namespace InvenireServer.Infrastructure.Email.Builders;

public class AdminEmailBuilder : BaseEmailBuilder, IAdminEmailBuilder
{
    public AdminEmailBuilder(string source) : base(source)
    {
    }

    public MailMessage BuildOrganizationCreationEmail(AdminOrganizationCreationEmailDto dto)
    {
        var message = new MailMessage(SourceAddress, dto.AdminAddress)
        {
            IsBodyHtml = true
        };

        message.Subject = "Successful organization creation!";
        message.Body = ParseHtmlTemplate(Path.Combine(AppContext.BaseDirectory, "assets", "templates", "admin_organization_creation_email_template.html"), dto);

        return message;
    }

    public MailMessage BuildVerificationEmail(AdminVerificationEmailDto dto)
    {
        var message = new MailMessage(SourceAddress, dto.AdminAddress)
        {
            IsBodyHtml = true
        };

        message.Subject = "Please verify your email to complete your registration";
        message.Body = ParseHtmlTemplate(Path.Combine(AppContext.BaseDirectory, "assets", "templates", "admin_verification_email_template.html"), dto);

        return message;
    }
}