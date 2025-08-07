using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class CreateOrganizationInvitationCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly CreateOrganizationInvitationCommandHandler _handler;

    public CreateOrganizationInvitationCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new CreateOrganizationInvitationCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectInvitationInstance()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);

        var command = new CreateOrganizationInvitationCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], []),
            EmployeeId = employee.Id,
            Description = new Faker().Lorem.Sentences(3)
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Employee!.Id == employee.Id && i.OrganizationId == organization.Id)).ReturnsAsync((OrganizationInvitation?)null);
        _repositories.Setup(r => r.Organizations.Invitations.CountAsync(i => i.OrganizationId == organization.Id)).ReturnsAsync(0);
        _repositories.Setup(r => r.Organizations.Update(It.IsAny<Organization>()));
        _repositories.Setup(r => r.Organizations.Invitations.Create(It.IsAny<OrganizationInvitation>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var result = await _handler.Handle(command, CancellationToken.None);

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
    public async Task Handle_ThrowsExceptionWhenTheAdminIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        var command = new CreateOrganizationInvitationCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], []),
            EmployeeId = employee.Id,
            Description = new Faker().Lorem.Sentences(3)
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheEmployeeIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);

        var command = new CreateOrganizationInvitationCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], []),
            EmployeeId = EmployeeFaker.Fake().Id,
            Description = new Faker().Lorem.Sentences(3)
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminDoesntOwnAnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null);

        var command = new CreateOrganizationInvitationCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], []),
            EmployeeId = employee.Id,
            Description = new Faker().Lorem.Sentences(3)
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Employee!.Id == employee.Id && i.OrganizationId == organization.Id)).ReturnsAsync((OrganizationInvitation?)null);
        _repositories.Setup(r => r.Organizations.Invitations.CountAsync(i => i.OrganizationId == organization.Id)).ReturnsAsync(0);
        _repositories.Setup(r => r.Organizations.Update(It.IsAny<Organization>()));
        _repositories.Setup(r => r.Organizations.Invitations.Create(It.IsAny<OrganizationInvitation>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenOrganizationAlreadyHasAInvitationForTheEmployee()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee: employee);
        var organization = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);

        var command = new CreateOrganizationInvitationCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], []),
            EmployeeId = employee.Id,
            Description = new Faker().Lorem.Sentences(3)
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Employee!.Id == employee.Id && i.OrganizationId == organization.Id)).ReturnsAsync(invitation);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage("The organization already has a invitation for the employee.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheEmployeeIsAlreadyPartOfTheOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, employees: [employee]);

        var command = new CreateOrganizationInvitationCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], []),
            EmployeeId = employee.Id,
            Description = new Faker().Lorem.Sentences(3)
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Employee!.Id == employee.Id && i.OrganizationId == organization.Id)).ReturnsAsync((OrganizationInvitation?)null);
        _repositories.Setup(r => r.Organizations.Invitations.CountAsync(i => i.OrganizationId == organization.Id)).ReturnsAsync(0);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage("The invitation is already part of another organization.");

    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheNumberOfOrganizationInvitationsExceedsMaximum()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);

        var command = new CreateOrganizationInvitationCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], []),
            EmployeeId = employee.Id,
            Description = new Faker().Lorem.Sentences(3)
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Employee!.Id == employee.Id && i.OrganizationId == organization.Id));
        _repositories.Setup(r => r.Organizations.Invitations.CountAsync(i => i.OrganizationId == organization.Id)).ReturnsAsync(Organization.MAX_INVITATIONS);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage($"The organization's number of invitations exceeded the limit (max {Organization.MAX_INVITATIONS})");
    }
}