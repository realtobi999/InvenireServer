using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Validators;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers;

namespace InvenireServer.Tests.Unit.Validators;

public class EmployeeValidatorTests
{
    private readonly Mock<IRepositoryManager> _repository;
    private readonly EmployeeValidator _validator;

    public EmployeeValidatorTests()
    {
        _repository = new Mock<IRepositoryManager>();
        _validator = new EmployeeValidator(_repository.Object);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenEmailIsNotUnique()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Mock the repository to return a employee that has seemingly the same email address.
        _repository.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == employee.EmailAddress && e.Id != employee.Id)).ReturnsAsync(new EmployeeFaker().Generate());

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(employee);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Employee.EmailAddress));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenUpdatedAtIsSetBeforeCreatedAt()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Set the updated at time to be before the created at time.
        employee.LastUpdatedAt = employee.CreatedAt.AddMonths(-1);

        _repository.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == employee.EmailAddress && e.Id != employee.Id)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(employee);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Employee.LastUpdatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenLastLoginAtIsSetBeforeCreatedAt()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Set the last login time to be before the created at time.
        employee.LastLoginAt = employee.CreatedAt.AddMonths(-1);

        _repository.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == employee.EmailAddress && e.Id != employee.Id)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(employee);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Employee.LastLoginAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenCreatedAtIsSetInTheFuture()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Set the created at time to be in the future.
        employee.CreatedAt = DateTimeOffset.UtcNow.AddMonths(1);
        employee.LastUpdatedAt = null;

        _repository.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == employee.EmailAddress && e.Id != employee.Id)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(employee);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Employee.CreatedAt));
    }
}