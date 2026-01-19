using System.Net.Mail;
using System.Text.RegularExpressions;
using InvenireServer.Application.Core.Admins.Commands.Verification.Send;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands.Verification;

/// <summary>
/// Tests for <see cref="SendVerificationAdminCommandHandler"/>.
/// </summary>
public class SendVerificationAdminCommandHandlerTests : CommandHandlerTester
{
    private readonly SendVerificationAdminCommandHandler _handler;

    public SendVerificationAdminCommandHandlerTests()
    {
        _handler = new SendVerificationAdminCommandHandler(_jwt, _email.Object, _repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler sends a verification email with a valid purpose claim.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new SendVerificationAdminCommand
        {
            Jwt = _jwt.Builder.Build([]),
            FrontendBaseAddress = "https://invenire.com"
        };

        admin.IsVerified = false;

        // Prepare - email.
        var dto = (AdminVerificationEmailDto?)null;
        _email.Setup(e => e.Builders.Admin.BuildVerificationEmail(It.IsAny<AdminVerificationEmailDto>())).Callback<AdminVerificationEmailDto>(captured => dto = captured);
        _email.Setup(e => e.Sender.SendEmailAsync(It.IsAny<MailMessage>()));

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        // Assert that the email is correctly build.
        dto.Should().NotBeNull();
        dto!.AdminAddress.Should().Be(admin.EmailAddress);
        dto!.AdminFirstName.Should().Be(admin.FirstName);

        // Assert that the token has all the added claims.
        var match = Regex.Match(dto.VerificationLink, @$"{command.FrontendBaseAddress}/verify-email\?token=([\w\-_.]+)");
        var jwt = JwtBuilder.Parse(match.Groups[1].Value);
        jwt.Payload.Should().Contain(c => c.Type == "purpose" && c.Value == "email_verification");
    }

    /// <summary>
    /// Verifies that the handler throws when the admin is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new SendVerificationAdminCommand
        {
            Jwt = _jwt.Builder.Build([]),
            FrontendBaseAddress = "https://invenire.com"
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    /// <summary>
    /// Verifies that the handler throws when the admin is already verified.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsAlreadyVerified()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new SendVerificationAdminCommand
        {
            Jwt = _jwt.Builder.Build([]),
            FrontendBaseAddress = "https://invenire.com"
        };

        admin.IsVerified = true;

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin's verification status is already confirmed.");
    }
}
