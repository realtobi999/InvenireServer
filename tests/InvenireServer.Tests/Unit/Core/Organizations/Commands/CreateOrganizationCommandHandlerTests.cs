using System.Net.Mail;
using InvenireServer.Application.Core.Organizations.Commands.Create;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

public class CreateOrganizationCommandHandlerTests
{
    private readonly Mock<IEmailManager> _email;
    private readonly CreateOrganizationCommandHandler _handler;
    private readonly Mock<IServiceManager> _services;

    public CreateOrganizationCommandHandlerTests()
    {
        _email = new Mock<IEmailManager>();
        _services = new Mock<IServiceManager>();
        _handler = new CreateOrganizationCommandHandler(_services.Object, _email.Object);
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

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.CreateAsync(It.IsAny<Organization>()));
        _services.Setup(s => s.Admins.UpdateAsync(It.IsAny<Admin>()));

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
}