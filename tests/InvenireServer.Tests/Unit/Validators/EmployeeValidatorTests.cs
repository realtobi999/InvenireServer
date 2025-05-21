using InvenireServer.Application.Core.Validators;
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
