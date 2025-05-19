using InvenireServer.Application.Core.Validators;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Exceptions.Http;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Tests.Integration.Fakers;
using Moq;

namespace InvenireServer.Tests.Unit.Validators;

public class EmployeeValidatorTests
{
    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenEmailIsNotUnique()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Mock the repository to return that the email address is not unique.
        var repository = new Mock<IRepositoryManager>();
        repository.Setup(r => r.Employee.IsEmailAddressUnique(employee.EmailAddress)).ReturnsAsync(false);
        var validator = new EmployeeValidator(repository.Object);

        // Act & Assert.
        var (valid, exception) = await validator.ValidateAsync(employee);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Be("Invalid value for EmailAddress: the address is already in use.");
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenLoginAttemptsHasInvalidValues()
    {
        // Prepare.
        var employee1 = new EmployeeFaker().Generate();
        var employee2 = new EmployeeFaker().Generate();

        // Set the login attempts negative.
        employee1.LoginAttempts = -1;
        // Set the login attempts above the maximum value.
        employee2.LoginAttempts = Employee.MAX_LOGIN_ATTEMPTS + 1;

        var repository = new Mock<IRepositoryManager>();
        repository.Setup(r => r.Employee.IsEmailAddressUnique(employee1.EmailAddress)).ReturnsAsync(true);
        repository.Setup(r => r.Employee.IsEmailAddressUnique(employee2.EmailAddress)).ReturnsAsync(true);
        var validator = new EmployeeValidator(repository.Object);

        // Act & Assert.
        foreach (var employee in new List<Employee> { employee1, employee2 })
        {
            var (valid, exception) = await validator.ValidateAsync(employee);

            valid.Should().BeFalse();
            exception.Should().BeOfType<BadRequest400Exception>();
            exception!.Message.Should().Be("Invalid value for LoginAttempts: must be between 0 and the maximum allowed.");
        }

    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenExpirationDateIsNullWhenLockIsSet()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Set the lock to be set and the expiration time to null.
        employee.LoginLock.IsSet = true;
        employee.LoginLock.ExpirationDate = null;

        var repository = new Mock<IRepositoryManager>();
        repository.Setup(r => r.Employee.IsEmailAddressUnique(employee.EmailAddress)).ReturnsAsync(true);
        var validator = new EmployeeValidator(repository.Object);

        // Act & Assert.
        var (valid, exception) = await validator.ValidateAsync(employee);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Be("Invalid value for LoginLock.ExpirationDate: must be provided when the lock is set.");
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenExpirationDateIsInThePastWhenLockIsSet()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Set the lock to be set and the expiration time to a time in the past.
        employee.LoginLock.IsSet = true;
        employee.LoginLock.ExpirationDate = DateTimeOffset.UtcNow.AddMonths(-1);

        var repository = new Mock<IRepositoryManager>();
        repository.Setup(r => r.Employee.IsEmailAddressUnique(employee.EmailAddress)).ReturnsAsync(true);
        var validator = new EmployeeValidator(repository.Object);

        // Act & Assert.
        var (valid, exception) = await validator.ValidateAsync(employee);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Be("Invalid value for LoginLock.ExpirationDate: must be set in the future.");
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenUpdatedAtIsSetBeforeCreatedAt()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Set the updated at time to be before the created at time.
        employee.UpdatedAt = employee.CreatedAt.AddMonths(-1);

        var repository = new Mock<IRepositoryManager>();
        repository.Setup(r => r.Employee.IsEmailAddressUnique(employee.EmailAddress)).ReturnsAsync(true);
        var validator = new EmployeeValidator(repository.Object);

        // Act & Assert.
        var (valid, exception) = await validator.ValidateAsync(employee);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Be("Invalid value for UpdatedAt: must be greater than CreatedAt.");
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenCreatedAtIsSetInTheFuture()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Set the created at time to be in the future.
        employee.CreatedAt = DateTimeOffset.UtcNow.AddMonths(1);
        employee.UpdatedAt = null;

        var repository = new Mock<IRepositoryManager>();
        repository.Setup(r => r.Employee.IsEmailAddressUnique(employee.EmailAddress)).ReturnsAsync(true);
        var validator = new EmployeeValidator(repository.Object);

        // Act & Assert.
        var (valid, exception) = await validator.ValidateAsync(employee);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Be("Invalid value for CreatedAt: cannot be set in the future.");
    }
}
