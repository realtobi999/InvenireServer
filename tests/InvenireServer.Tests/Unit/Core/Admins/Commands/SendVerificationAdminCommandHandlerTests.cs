using System.Net.Mail;
using System.Text.RegularExpressions;
using InvenireServer.Application.Core.Admins.Commands.Verification.Send;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class SendVerificationAdminCommandHandlerTests
{
    private readonly Mock<IEmailManager> _email;
    private readonly SendVerificationAdminCommandHandler _handler;
    private readonly Mock<IServiceManager> _services;

    public SendVerificationAdminCommandHandlerTests()
    {
        _email = new Mock<IEmailManager>();
        _services = new Mock<IServiceManager>();

        _handler = new SendVerificationAdminCommandHandler(_services.Object, _email.Object, JwtManagerFaker.Initiate());
    }

    [Fact]
    public async Task Handle_BuildsCorrectToken()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new SendVerificationAdminCommand
        {
            Jwt = new Jwt([], []),
            FrontendBaseUrl = "invenire.com"
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);

        var dto = (AdminVerificationEmailDto?)null;
        _email.Setup(e => e.Builders.Admin.BuildVerificationEmail(It.IsAny<AdminVerificationEmailDto>())).Callback<AdminVerificationEmailDto>(captured => dto = captured);
        _email.Setup(e => e.Sender.SendEmailAsync(It.IsAny<MailMessage>()));

        // Act & Assert
        await _handler.Handle(command, CancellationToken.None);

        // Assert that the email dto is correctly build.
        dto.Should().NotBeNull();
        dto!.AdminAddress.Should().Be(admin.EmailAddress);
        dto!.AdminName.Should().Be(admin.Name);

        // Assert that the token has all the added claims.
        var match = Regex.Match(dto.VerificationLink, $@"{command.FrontendBaseUrl}/verify-email\?token=([\w\-_.]+)");
        var jwt = JwtBuilder.Parse(match.Groups[1].Value);
        jwt.Payload.Should().Contain(c => c.Type == "purpose" && c.Value == "email_verification");
    }
}