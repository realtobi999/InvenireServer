using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Infrastructure.Email;

/// <summary>
/// Creates email sender instances from configuration.
/// </summary>
public static class EmailSenderFactory
{
    /// <summary>
    /// Creates and configures an <see cref="EmailSender"/> using SMTP settings.
    /// </summary>
    /// <param name="configuration">Configuration containing SMTP settings.</param>
    /// <returns>Configured email sender.</returns>
    public static EmailSender Initiate(IConfiguration configuration)
    {
        var host = configuration.GetSection("SMTP:Host").Value ?? throw new NullReferenceException();
        var port = int.Parse(configuration.GetSection("SMTP:Port").Value ?? throw new NullReferenceException());

        var client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        var username = configuration.GetSection("SMTP:Username").Value ?? throw new NullReferenceException();
        var password = configuration.GetSection("SMTP:Password").Value ?? throw new NullReferenceException();

        client.Credentials = new NetworkCredential(username, password);

        return new EmailSender(client, username);
    }
}
