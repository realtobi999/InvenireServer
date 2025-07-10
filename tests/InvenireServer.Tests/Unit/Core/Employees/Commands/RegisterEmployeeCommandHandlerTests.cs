using InvenireServer.Application.Core.Employees.Commands.Register;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Integration.Extensions;
using InvenireServer.Tests.Integration.Fakers.Common;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class RegisterEmployeeCommandHandlerTests
{
    private readonly RegisterEmployeeCommandHandler _handler;
    private readonly Mock<IServiceManager> _services;

    public RegisterEmployeeCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new RegisterEmployeeCommandHandler(_services.Object, new PasswordHasher<Employee>(), new JwtManagerFaker().Initiate());
    }

    [Fact]
    public async Task Handle_ReturnsCorrectTokenAndEmployeeInstance()
    {
        // Prepare.
        var faker = new Faker();
        var password = faker.Internet.SecurePassword();
        var command = new RegisterEmployeeCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Internet.UserName(),
            EmailAddress = faker.Internet.Email(),
            Password = password,
            PasswordConfirm = password
        };

        _services.Setup(s => s.Employees.CreateAsync(It.IsAny<Employee>()));

        // Act & Assert.
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert that the employee is correctly constructed.
        var employee = result.Employee;
        employee.Id.Should().Be(command.Id.ToString());
        employee.Name.Should().Be(command.Name);
        employee.EmailAddress.Should().Be(command.EmailAddress);
        employee.Password.Should().NotBe(password);
        employee.IsVerified.Should().BeFalse();
        employee.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        employee.LastLoginAt.Should().BeNull();
        employee.LastUpdatedAt.Should().BeNull();

        // Assert that the token has all the necessary claims.
        var jwt = JwtBuilder.Parse(result.Token);
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.EMPLOYEE);
        jwt.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == command.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.FalseString);
    }
}