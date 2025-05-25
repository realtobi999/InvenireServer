using Moq;
using InvenireServer.Tests.Integration.Fakers;
using InvenireServer.Domain.Core.Exceptions.Http;
using InvenireServer.Application.Core.Validators;
using InvenireServer.Domain.Core.Interfaces.Managers;

namespace InvenireServer.Tests.Unit.Validators;

public class EmployeeValidatorTests
{
    private readonly EmployeeValidator _validator;
    private readonly Mock<IRepositoryManager> _repository;

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

        // Mock the repository to return that the email address is not unique.
        _repository.Setup(r => r.Employee.IsEmailAddressUnique(employee.EmailAddress)).ReturnsAsync(false);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(employee);

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

        _repository.Setup(r => r.Employee.IsEmailAddressUnique(employee.EmailAddress)).ReturnsAsync(true);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(employee);

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

        _repository.Setup(r => r.Employee.IsEmailAddressUnique(employee.EmailAddress)).ReturnsAsync(true);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(employee);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Be("Invalid value for CreatedAt: cannot be set in the future.");
    }
}
