using System.Net.Mail;
using InvenireServer.Application.Core.Organizations.Commands.Create;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

public class CreateOrganizationCommandHandlerTests : CommandHandlerTester
{
    private readonly CreateOrganizationCommandHandler _handler;

    public CreateOrganizationCommandHandlerTests()
    {
        _handler = new CreateOrganizationCommandHandler(_email.Object, _repositories.Object);
    }

    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new CreateOrganizationCommand
        {
            Name = organization.Name,
            Jwt = _jwt.Builder.Build([]),
            FrontendBaseAddress = "https://www.invenire.com",
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Admins.Update(admin));
        _repositories.Setup(r => r.Organizations.Create(organization));
        _repositories.Setup(r => r.SaveOrThrowAsync()).Returns(Task.CompletedTask);

        // Prepare - email.
        _email.Setup(e => e.Builders.Admin.BuildOrganizationCreationEmail(It.IsAny<AdminOrganizationCreationEmailDto>()));
        _email.Setup(e => e.Sender.SendEmailAsync(It.IsAny<MailMessage>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        var result = await action.Invoke();
        result.Organization.Should().NotBeNull();
        result.Organization.Id.Should().NotBeEmpty();
        result.Organization.Name.Should().Be(organization.Name);
        result.Organization.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        result.Organization.LastUpdatedAt.Should().BeNull();

        // Assert that the admin is assigned to the organization.
        admin.OrganizationId.Should().Be(result.Organization.Id);
    }
}
