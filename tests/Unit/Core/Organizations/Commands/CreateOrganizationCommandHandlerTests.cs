using System.Net.Mail;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Core.Organizations.Commands.Create;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

public class CreateOrganizationCommandHandlerTests
{
    private readonly Mock<IEmailManager> _email;
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly CreateOrganizationCommandHandler _handler;

    public CreateOrganizationCommandHandlerTests()
    {
        _email = new Mock<IEmailManager>();
        _repositories = new Mock<IRepositoryManager>();
        _handler = new CreateOrganizationCommandHandler(_email.Object, _repositories.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectOrganizationInstanceAndEmailDto()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var faker = new Faker();
        var command = new CreateOrganizationCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Internet.UserName(),
            Jwt = new Jwt([], []),
            FrontendBaseUrl = "invenire.com"
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Admins.Update(It.IsAny<Admin>()));
        _repositories.Setup(r => r.Organizations.Create(It.IsAny<Organization>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        var dto = (AdminOrganizationCreationEmailDto?)null;
        _email.Setup(e => e.Builders.Admin.BuildOrganizationCreationEmail(It.IsAny<AdminOrganizationCreationEmailDto>())).Callback<AdminOrganizationCreationEmailDto>(captured => dto = captured);
        _email.Setup(e => e.Sender.SendEmailAsync(It.IsAny<MailMessage>()));

        // Act & Assert.
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert that the organization is correctly constructed.
        var organization = result.Organization;
        organization.Id.Should().Be(command.Id.ToString());
        organization.Name.Should().Be(command.Name);
        organization.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        organization.LastUpdatedAt.Should().BeNull();

        // Assert that the admin is the owner of the organization
        organization.Admin.Should().NotBeNull();
        organization.Admin!.Id.Should().Be(admin.Id);
        admin.OrganizationId.Should().Be(organization.Id);

        // Assert that the email dto is correctly build.
        dto.Should().NotBeNull();
        dto!.AdminAddress.Should().Be(admin.EmailAddress);
        dto!.AdminName.Should().Be(admin.Name);
        dto!.OrganizationName.Should().Be(organization.Name);
        dto!.DashboardLink.Should().Be($"{command.FrontendBaseUrl}/dashboard");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminIsNotFound()
    {
        // Prepare.
        var faker = new Faker();
        var command = new CreateOrganizationCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Internet.UserName(),
            Jwt = new Jwt([], []),
            FrontendBaseUrl = "invenire.com"
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }
}