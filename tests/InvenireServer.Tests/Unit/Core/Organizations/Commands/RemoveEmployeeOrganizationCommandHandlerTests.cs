using System.Linq.Expressions;
using InvenireServer.Application.Core.Organizations.Commands.RemoveEmployee;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Properties.Items;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

/// <summary>
/// Tests for <see cref="RemoveEmployeeOrganizationCommandHandler"/>.
/// </summary>
public class RemoveEmployeeOrganizationCommandHandlerTests : CommandHandlerTester
{
    private readonly RemoveEmployeeOrganizationCommandHandler _handler;

    public RemoveEmployeeOrganizationCommandHandlerTests()
    {
        _handler = new RemoveEmployeeOrganizationCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler removes an employee from the organization.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (int i = 0; i < 20; i++)
            items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake(items: items);
        var property = PropertyFaker.Fake(items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);
        var command = new RemoveEmployeeOrganizationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            EmployeeId = employee.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Items.IndexAsync(new QueryOptions<PropertyItem, PropertyItemDto>
        {
            Filtering = new QueryFilteringOptions<PropertyItem>
            {
                Filters = new List<Expression<Func<PropertyItem, bool>>?>
                {
                    i => i.PropertyId == property.Id && i.EmployeeId == employee.Id
                }
            }
        })).ReturnsAsync(items.Select(i => new PropertyItemDto { Id = i.Id, EmployeeId = i.EmployeeId }));
        _repositories.Setup(r => r.Employees.Update(employee));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        employee.OrganizationId.Should().BeNull();
    }


    /// <summary>
    /// Verifies that the handler throws when the admin is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var command = new RemoveEmployeeOrganizationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            EmployeeId = employee.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }


    /// <summary>
    /// Verifies that the handler throws when the employee is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.

        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var command = new RemoveEmployeeOrganizationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            EmployeeId = employee.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }


    /// <summary>
    /// Verifies that the handler throws when the admin does not own an organization.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, employees: [employee]);
        var command = new RemoveEmployeeOrganizationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            EmployeeId = employee.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }
}
