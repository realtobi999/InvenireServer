using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Validators;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers;

namespace InvenireServer.Tests.Unit.Validators;

public class AdminValidatorTests
{
    private readonly Mock<IRepositoryManager> _repository;
    private readonly AdminValidator _validator;

    public AdminValidatorTests()
    {
        _repository = new Mock<IRepositoryManager>();
        _validator = new AdminValidator(_repository.Object);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenEmailIsNotUnique()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        // Mock the repository to return a employee that has seemingly the same email address.
        _repository.Setup(r => r.Admins.GetAsync(e => e.EmailAddress == admin.EmailAddress && e.Id != admin.Id)).ReturnsAsync(new AdminFaker().Generate());

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(admin);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Employee.EmailAddress));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenUpdatedAtIsSetBeforeCreatedAt()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        // Set the updated at time to be before the created at time.
        admin.LastUpdatedAt = admin.CreatedAt.AddMonths(-1);

        _repository.Setup(r => r.Admins.GetAsync(e => e.EmailAddress == admin.EmailAddress && e.Id != admin.Id)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(admin);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Employee.LastUpdatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenLastLoginAtIsSetBeforeCreatedAt()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        // Set the last login time to be before the created at time.
        admin.LastLoginAt = admin.CreatedAt.AddMonths(-1);

        _repository.Setup(r => r.Admins.GetAsync(e => e.EmailAddress == admin.EmailAddress && e.Id != admin.Id)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(admin);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Employee.LastLoginAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenCreatedAtIsSetInTheFuture()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        // Set the created at time to be in the future.
        admin.CreatedAt = DateTimeOffset.UtcNow.AddMonths(1);
        admin.LastUpdatedAt = null;

        _repository.Setup(r => r.Admins.GetAsync(e => e.EmailAddress == admin.EmailAddress && e.Id != admin.Id)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(admin);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Employee.CreatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenOrganizationIsNotFound()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();

        _repository.Setup(r => r.Admins.GetAsync(e => e.EmailAddress == admin.EmailAddress && e.Id != admin.Id)).ReturnsAsync((Admin?)null);
        _repository.Setup(r => r.Organizations.GetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(admin);

        valid.Should().BeFalse();
        exception.Should().BeOfType<NotFound404Exception>();
        exception!.Message.Should().Contain(nameof(Organization));
    }
}