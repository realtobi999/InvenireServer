using System.Security.Claims;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Delete;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class DeleteOrganizationInvitationCommandHandlerTests : CommandHandlerTester
{
    private readonly DeleteOrganizationInvitationCommandHandler _handler;

    public DeleteOrganizationInvitationCommandHandlerTests()
    {
        _handler = new DeleteOrganizationInvitationCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_ThrowsNoException_AsAdmin()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            Id = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.Update(organization));
        _repositories.Setup(r => r.Organizations.Invitations.Delete(invitation));
        _repositories.Setup(r => r.SaveOrThrowAsync()).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        invitation.OrganizationId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            Id = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            Id = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationIsNotFound_ForAdmin()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            Id = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync((OrganizationInvitation?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The invitation was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationIsNotPartOfOrganization_ForAdmin()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        invitation.AssignOrganization(OrganizationFaker.Fake());
        var organization = OrganizationFaker.Fake(admin: admin);
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            Id = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync(invitation);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The invitation isn't part of the organization.");
    }

    [Fact]
    public async Task Handle_ThrowsNoException_AsEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee);
        var organization = OrganizationFaker.Fake(invitations: [invitation]);
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            Id = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Update(organization));
        _repositories.Setup(r => r.Organizations.Invitations.Delete(invitation));
        _repositories.Setup(r => r.SaveOrThrowAsync()).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        invitation.OrganizationId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            Id = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationIsNotFound_ForEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            Id = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync((OrganizationInvitation?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The invitation was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotFound_ForEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee);
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            Id = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The organization assigned to the invitation was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationIsNotAssignedToEmployee_ForEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var otherEmployee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(otherEmployee);
        var organization = OrganizationFaker.Fake(invitations: [invitation]);
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            Id = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The invitation doesn't belong to the employee.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationIsNotPartOfOrganization_ForEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee);
        invitation.AssignOrganization(OrganizationFaker.Fake());
        var organization = OrganizationFaker.Fake();
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            Id = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The invitation isn't part of the organization.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenRoleIsUnauthorized()
    {
        // Prepare.
        var command = new DeleteOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", "OTHER")]),
            Id = Guid.NewGuid(),
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("You are not authorized to perform the action. Please ensure you have the necessary permissions.");
    }
}
