using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Validators.Organizations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Validators;

public class OrganizationValidatorTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly OrganizationValidator _validator;

    public OrganizationValidatorTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _validator = new OrganizationValidator(_repositories.Object);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenEmailIsNotUnique()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();

        // Mock the repository to return a organization that has seemingly the same name.
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Name == organization.Name && o.Id != organization.Id)).ReturnsAsync(new OrganizationFaker().Generate());

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(organization);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Organization.Name));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenUpdatedAtIsSetBeforeCreatedAt()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();

        // Set the updated at time to be before the created at time.
        organization.LastUpdatedAt = organization.CreatedAt.AddMonths(-1);

        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Name == organization.Name && o.Id != organization.Id)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(organization);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Organization.LastUpdatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenCreatedAtIsSetInTheFuture()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();

        // Set the created at time to be in the future.
        organization.CreatedAt = DateTimeOffset.UtcNow.AddMonths(1);
        organization.LastUpdatedAt = null;

        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Name == organization.Name && o.Id != organization.Id)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(organization);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Organization.CreatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenAdminIsNotAssigned()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();

        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Name == organization.Name && o.Id != organization.Id)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(organization);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Admin));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenAdminIsNotFound()
    {
        // Prepare
        var admin = new AdminFaker().Generate();
        var organization = new OrganizationFaker().Generate();

        organization.Admin = admin;

        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Name == organization.Name && o.Id != organization.Id)).ReturnsAsync((Organization?)null);
        // Mock the repository so it doesnt return the admin from the repository.
        _repositories.Setup(r => r.Admins.GetAsync(a => a.Id == organization.Admin.Id)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(organization);

        valid.Should().BeFalse();
        exception.Should().BeOfType<NotFound404Exception>();
        exception!.Message.Should().Contain(nameof(Admin));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenEmployeeIsNotFound()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();
        var employee = new EmployeeFaker().Generate();
        var organization = new OrganizationFaker().Generate();

        organization.Admin = admin;
        organization.Employees = [employee];

        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Name == organization.Name && o.Id != organization.Id)).ReturnsAsync((Organization?)null);
        _repositories.Setup(r => r.Admins.GetAsync(a => a.Id == organization.Admin.Id)).ReturnsAsync(admin);
        // Mock the repository so it doesnt return the employee from the repository.
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == employee.Id)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(organization);

        valid.Should().BeFalse();
        exception.Should().BeOfType<NotFound404Exception>();
        exception!.Message.Should().Contain(nameof(Employee));
    }
}