using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class CreateOrganizationInvitationCommandTests : CommandHandlerTester
{
    private readonly CreateOrganizationInvitationCommandHandler _handler;

    public CreateOrganizationInvitationCommandTests()
    {
        _handler = new CreateOrganizationInvitationCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new CreateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentences(),
            EmployeeId = employee.Id,
            EmployeeEmailAddress = employee.EmailAddress,
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == command.EmployeeEmailAddress)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.CountAsync(i => i.Employee!.Id == employee.Id && i.OrganizationId == organization.Id)).ReturnsAsync(0);
        _repositories.Setup(r => r.Organizations.Invitations.CountAsync(i => i.OrganizationId == organization.Id)).ReturnsAsync(0);
        _repositories.Setup(r => r.Organizations.Update(organization));
        _repositories.Setup(r => r.Organizations.Invitations.Create(It.IsAny<OrganizationInvitation>()));
        _repositories.Setup(r => r.SaveOrThrowAsync()).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        var result = await action.Invoke();
        result.Invitation.Should().NotBeNull();
        result.Invitation.Id.Should().NotBeEmpty();
        result.Invitation.Description.Should().Be(command.Description);
        result.Invitation.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        result.Invitation.LastUpdatedAt.Should().BeNull();

        // Assert that the invitation is assigned to the organization.
        result.Invitation.OrganizationId.Should().Be(organization.Id);
        // Assert that the employee is assigned to the invitation.
        result.Invitation.Employee.Should().NotBeNull();
        result.Invitation.Employee!.Id.Should().Be(employee.Id);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound_ById()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new CreateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentences(),
            EmployeeId = employee.Id,
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound_ByEmail()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new CreateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentences(),
            EmployeeEmailAddress = employee.EmailAddress,
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == command.EmployeeEmailAddress)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new CreateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentences(),
            EmployeeId = employee.Id,
            EmployeeEmailAddress = employee.EmailAddress,
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == command.EmployeeEmailAddress)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var admin = AdminFaker.Fake();
        var command = new CreateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentences(),
            EmployeeId = employee.Id,
            EmployeeEmailAddress = employee.EmailAddress,
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == command.EmployeeEmailAddress)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationAlreadyExistsForEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new CreateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentences(),
            EmployeeId = employee.Id,
            EmployeeEmailAddress = employee.EmailAddress,
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == command.EmployeeEmailAddress)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.CountAsync(i => i.Employee!.Id == employee.Id && i.OrganizationId == organization.Id)).ReturnsAsync(1);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage("The organization already has a invitation for the employee.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationInvitationLimitIsExceeded()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new CreateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentences(),
            EmployeeId = employee.Id,
            EmployeeEmailAddress = employee.EmailAddress,
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == command.EmployeeEmailAddress)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.CountAsync(i => i.Employee!.Id == employee.Id && i.OrganizationId == organization.Id)).ReturnsAsync(0);
        _repositories.Setup(r => r.Organizations.Invitations.CountAsync(i => i.OrganizationId == organization.Id)).ReturnsAsync(Organization.MAX_INVITATIONS + 1);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage($"The organization's number of invitations exceeded the limit (max {Organization.MAX_INVITATIONS}).");
    }
}
