using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Infrastructure.Email;

/// <summary>
/// Factory for creating an instance of <c>EmailSender</c> using SMTP configuration values.
/// </summary>
public static class EmailSenderFactory
{
    /// <summary>
    /// Creates and configures an <c>EmailSender</c> instance using SMTP settings from the application configuration.
    /// </summary>
    /// <param name="configuration">The application configuration containing SMTP settings.</param>
    /// <returns>A configured instance of <c>EmailSender</c>.</returns>
    /// <exception cref="NullReferenceException">
    /// Thrown when any of the required SMTP settings (Host, Port, Username, or Password) are missing from the configuration.
    /// </exception>
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