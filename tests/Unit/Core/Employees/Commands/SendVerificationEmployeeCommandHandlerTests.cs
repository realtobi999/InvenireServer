using System.Net.Mail;
using System.Text.RegularExpressions;
using InvenireServer.Application.Core.Employees.Commands.Verification.Send;
using InvenireServer.Application.Dtos.Employees.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Fakers.Common;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class SendVerificationEmployeeCommandHandlerTests
{
    private readonly Mock<IEmailManager> _email;
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly SendVerificationEmployeeCommandHandler _handler;

    public SendVerificationEmployeeCommandHandlerTests()
    {
        _email = new Mock<IEmailManager>();
        _repositories = new Mock<IRepositoryManager>();
        _handler = new SendVerificationEmployeeCommandHandler(JwtManagerFaker.Initiate(), _email.Object, _repositories.Object);
    }

    [Fact]
    public async Task Handle_BuildsCorrectToken()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new SendVerificationEmployeeCommand
        {
            Jwt = new Jwt([], []),
            FrontendBaseAddress = "invenire.com"
        };

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);

        var dto = (EmployeeVerificationEmailDto?)null;
        _email.Setup(e => e.Builders.Employee.BuildVerificationEmail(It.IsAny<EmployeeVerificationEmailDto>())).Callback<EmployeeVerificationEmailDto>(captured => dto = captured);
        _email.Setup(e => e.Sender.SendEmailAsync(It.IsAny<MailMessage>()));

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the email dto is correctly build.
        dto.Should().NotBeNull();
        dto!.EmployeeAddress.Should().Be(employee.EmailAddress);
        dto!.EmployeeName.Should().Be(employee.Name);

        // Assert that the token has all the added claims.
        var match = Regex.Match(dto.VerificationLink, $@"{command.FrontendBaseAddress}/verify-email\?token=([\w\-_.]+)");
        var jwt = JwtBuilder.Parse(match.Groups[1].Value);
        jwt.Payload.Should().Contain(c => c.Type == "purpose" && c.Value == "email_verification");
    }
}