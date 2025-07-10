using System.Net.Mail;
using System.Text.RegularExpressions;
using InvenireServer.Application.Core.Employees.Commands.Verification.Send;
using InvenireServer.Application.Dtos.Employees.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class SendVerificationEmployeeCommandHandlerTests
{
    private readonly Mock<IEmailManager> _email;
    private readonly Mock<IServiceManager> _services;
    private readonly SendVerificationEmployeeCommandHandler _handler;

    public SendVerificationEmployeeCommandHandlerTests()
    {
        _email = new Mock<IEmailManager>();
        _services = new Mock<IServiceManager>();

        _handler = new SendVerificationEmployeeCommandHandler(_services.Object, _email.Object, JwtManagerFaker.Initiate());
    }

    [Fact]
    public async Task Handle_BuildsCorrectToken()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new SendVerificationEmployeeCommand
        {
            Jwt = new Jwt([], []),
            FrontendBaseUrl = "invenire.com"
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);

        var dto = (EmployeeVerificationEmailDto?)null;
        _email.Setup(e => e.Builders.Employee.BuildVerificationEmail(It.IsAny<EmployeeVerificationEmailDto>())).Callback<EmployeeVerificationEmailDto>(captured => dto = captured);
        _email.Setup(e => e.Sender.SendEmailAsync(It.IsAny<MailMessage>()));

        // Act & Assert
        await _handler.Handle(command, CancellationToken.None);

        // Assert that the email dto is correctly build.
        dto.Should().NotBeNull();
        dto!.EmployeeAddress.Should().Be(employee.EmailAddress);
        dto!.EmployeeName.Should().Be(employee.Name);

        // Assert that the token has all the added claims.
        var match = Regex.Match(dto.VerificationLink, $@"{command.FrontendBaseUrl}/verify-email\?token=([\w\-_.]+)");
        var jwt = JwtBuilder.Parse(match.Groups[1].Value);
        jwt.Payload.Should().Contain(c => c.Type == "purpose" && c.Value == "email_verification");
    }
}