using System.Net.Mail;
using System.Text.RegularExpressions;
using InvenireServer.Application.Core.Employees.Commands.Verification.Send;
using InvenireServer.Application.Dtos.Employees.Email;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands.Verification;

/// <summary>
/// Tests for <see cref="SendVerificationEmployeeCommandHandler"/>.
/// </summary>
public class SendVerificationEmployeeCommandHandlerTests : CommandHandlerTester
{
    private readonly SendVerificationEmployeeCommandHandler _handler;

    public SendVerificationEmployeeCommandHandlerTests()
    {
        _handler = new SendVerificationEmployeeCommandHandler(_jwt, _email.Object, _repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler sends a verification email with a valid purpose claim.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new SendVerificationEmployeeCommand
        {
            Jwt = _jwt.Builder.Build([]),
            FrontendBaseAddress = "https://invenire.com"
        };

        employee.IsVerified = false;

        // Prepare - email.
        var dto = (EmployeeVerificationEmailDto?)null;
        _email.Setup(e => e.Builders.Employee.BuildVerificationEmail(It.IsAny<EmployeeVerificationEmailDto>())).Callback<EmployeeVerificationEmailDto>(captured => dto = captured);
        _email.Setup(e => e.Sender.SendEmailAsync(It.IsAny<MailMessage>()));

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        // Assert that the email is correctly build.
        dto.Should().NotBeNull();
        dto!.EmployeeAddress.Should().Be(employee.EmailAddress);
        dto!.EmployeeFirstName.Should().Be(employee.FirstName);

        // Assert that the token has all the added claims.
        var match = Regex.Match(dto.VerificationLink, @$"{command.FrontendBaseAddress}/verify-email\?token=([\w\-_.]+)");
        var jwt = JwtBuilder.Parse(match.Groups[1].Value);
        jwt.Payload.Should().Contain(c => c.Type == "purpose" && c.Value == "email_verification");
    }

    /// <summary>
    /// Verifies that the handler throws when the employee is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new SendVerificationEmployeeCommand
        {
            Jwt = _jwt.Builder.Build([]),
            FrontendBaseAddress = "https://invenire.com"
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }

    /// <summary>
    /// Verifies that the handler throws when the employee is already verified.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsAlreadyVerified()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new SendVerificationEmployeeCommand
        {
            Jwt = _jwt.Builder.Build([]),
            FrontendBaseAddress = "https://invenire.com"
        };

        employee.IsVerified = true;

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The employee's verification status is already confirmed.");
    }
}
