using InvenireServer.Application.Core.Employees.Commands.Register;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Extensions;
using InvenireServer.Tests.Fakers.Common;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class RegisterEmployeeCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly IPasswordHasher<Employee> _hasher;
    private readonly RegisterEmployeeCommandHandler _handler;

    public RegisterEmployeeCommandHandlerTests()
    {
        _hasher = new PasswordHasher<Employee>();
        _repositories = new Mock<IRepositoryManager>();
        _handler = new RegisterEmployeeCommandHandler(JwtManagerFaker.Initiate(), _hasher, _repositories.Object);
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

        _repositories.Setup(r => r.Employees.Create(It.IsAny<Employee>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

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
        var jwt = JwtBuilder.Parse(result.Response.Token);
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.EMPLOYEE);
        jwt.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == command.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.FalseString);
    }
}