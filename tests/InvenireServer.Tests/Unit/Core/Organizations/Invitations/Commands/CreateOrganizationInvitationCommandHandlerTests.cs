using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class CreateOrganizationInvitationCommandHandlerTests
{
    private readonly CreateOrganizationInvitationCommandHandler _handler;
    private readonly Mock<IServiceManager> _services;

    public CreateOrganizationInvitationCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new CreateOrganizationInvitationCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectInvitationInstance()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();
        var employee = new EmployeeFaker(organization).Generate();

        // Assign the admin to the organization.
        organization.Admin = admin;

        var command = new CreateOrganizationInvitationCommand
        {
            Id = Guid.NewGuid(),
            Description = new Faker().Lorem.Sentences(3),
            EmployeeId = employee.Id,
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.CreateAsync(It.IsAny<OrganizationInvitation>()));
        _services.Setup(s => s.Organizations.UpdateAsync(organization));

        // Act & Assert.
        var result = await _handler.Handle(command, new CancellationToken());

        // Assert that the invitation is correctly constructed.
        var invitation = result.Invitation;
        invitation.Id.Should().Be(command.Id.ToString());
        invitation.Description.Should().Be(command.Description);
        invitation.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        invitation.LastUpdatedAt.Should().BeNull();
        invitation.Employee.Should().NotBeNull();
        invitation.Employee!.Id.Should().Be(employee.Id);
        invitation.OrganizationId.Should().Be(organization.Id);

        // Assert that the organization has the invitation.
        organization.Invitations.Should().Contain(i => i.Id == invitation.Id);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminDoesntOwnAnOrganization()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker().Generate();
        var employee = new EmployeeFaker(organization).Generate();

        var command = new CreateOrganizationInvitationCommand
        {
            Id = Guid.NewGuid(),
            Description = new Faker().Lorem.Sentences(3),
            EmployeeId = employee.Id,
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync((Organization?)null);
        _services.Setup(s => s.Organizations.Invitations.CreateAsync(It.IsAny<OrganizationInvitation>()));
        _services.Setup(s => s.Organizations.UpdateAsync(organization));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created an organization. You must first create an organization before creating invitations.");
    }
}