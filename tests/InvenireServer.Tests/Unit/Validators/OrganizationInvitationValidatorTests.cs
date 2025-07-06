using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Validators.Organizations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Validators;

public class OrganizationInvitationValidatorTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly OrganizationInvitationValidator _validator;

    public OrganizationInvitationValidatorTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _validator = new OrganizationInvitationValidator(_repositories.Object);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenUpdatedAtIsSetBeforeCreatedAt()
    {
        // Prepare.
        var invitation = new OrganizationInvitationFaker().Generate();

        // Set the updated at time to be before the created at time.
        invitation.LastUpdatedAt = invitation.CreatedAt.AddMonths(-1);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(invitation);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(OrganizationInvitation.LastUpdatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenCreatedAtIsSetInTheFuture()
    {
        // Prepare.
        var invitation = new OrganizationInvitationFaker().Generate();

        // Set the created at time to be in the future.
        invitation.CreatedAt = DateTimeOffset.UtcNow.AddMonths(1);
        invitation.LastUpdatedAt = null;

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(invitation);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(OrganizationInvitation.CreatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenOrganizationIsNotAssigned()
    {
        // Prepare.
        var invitation = new OrganizationInvitationFaker().Generate();

        // Set the organization id as null.
        invitation.OrganizationId = null;

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(invitation);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Organization));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenOrganizationIsNotFound()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var employee = new EmployeeFaker(organization).Generate();
        var invitation = new OrganizationInvitationFaker(organization, employee).Generate();

        // Mock the repository so it doesnt return the organization from the repository.
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(invitation);

        valid.Should().BeFalse();
        exception.Should().BeOfType<NotFound404Exception>();
        exception!.Message.Should().Contain(nameof(Organization));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenEmployeeIsNotAssigned()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var employee = new EmployeeFaker(organization).Generate();
        var invitation = new OrganizationInvitationFaker(organization, employee).Generate();

        // Set the employee as null.
        invitation.Employee = null;

        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(invitation);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Employee));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenEmployeeIsNotFound()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var employee = new EmployeeFaker(organization).Generate();
        var invitation = new OrganizationInvitationFaker(organization, employee).Generate();

        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);
        // Mock the repository so it doesnt return the employee from the repository.
        _repositories.Setup(r => r.Employees.GetAsync(o => o.Id == invitation.Employee!.Id)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(invitation);

        valid.Should().BeFalse();
        exception.Should().BeOfType<NotFound404Exception>();
        exception!.Message.Should().Contain(nameof(Employee));
    }
}