using System.Net.Mail;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Email.Builders;

namespace InvenireServer.Infrastructure.Email.Builders;

public class AdminEmailBuilder : BaseEmailBuilder, IAdminEmailBuilder
{
    public AdminEmailBuilder(string source) : base(source)
    {
    }

    public MailMessage BuildVerificationEmail(AdminVerificationEmailDto dto)
    {
        var message = new MailMessage(this.SourceAddress, dto.AdminAddress)
        {
            IsBodyHtml = true
        };

        message.Subject = "Please verify your email to complete your registration";
        message.Body = this.ParseHtmlTemplate(Path.Combine(AppContext.BaseDirectory, "assets", "templates", "admin_verification_email_template.html"), dto);

        return message;
    }
}
