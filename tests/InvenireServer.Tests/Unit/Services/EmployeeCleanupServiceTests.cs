using Moq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Application.Services;
using InvenireServer.Tests.Integration.Fakers;
using InvenireServer.Domain.Core.Interfaces.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace InvenireServer.Tests.Unit.Services;

public class EmployeeCleanupServiceTests
{
    private readonly Mock<IServiceScope> _mockServiceScope = new();
    private readonly Mock<IRepositoryManager> _mockRepositoryManager = new();
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory = new();
    private readonly Mock<ILogger<EmployeeCleanupService>> _mockLogger = new();

    private EmployeeCleanupService CreateService()
    {
        _mockServiceScope.Setup(ss => ss.ServiceProvider.GetService(typeof(IRepositoryManager))).Returns(_mockRepositoryManager.Object);
        _mockServiceScopeFactory.Setup(ssf => ssf.CreateScope()).Returns(_mockServiceScope.Object);

        return new EmployeeCleanupService(_mockServiceScopeFactory.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CleanupAsync_Removes_Unverified_Old_Employees()
    {
        // Prepare.
        var now = DateTimeOffset.UtcNow;
        var employees = new List<Employee> { };

        // Create employees that are categorized for deletion.
        for (int i = 0; i < 2; i++)
        {
            var employee = new EmployeeFaker().Generate();

            employee.IsVerified = false;
            employee.CreatedAt = now.AddDays(-8);

            employees.Add(employee);
        }

        _mockRepositoryManager
            .Setup(repo => repo.Employees.IndexAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
            .ReturnsAsync(employees);

        _mockRepositoryManager
            .Setup(m => m.SaveAsync())
            .ReturnsAsync(1);

        var service = CreateService();

        // Act & Assert.
        await service.CleanupAsync();

        foreach (var employee in employees)
        {
            _mockRepositoryManager.Verify(repo => repo.Employees.Delete(employee), Times.Once);
        }

        _mockRepositoryManager.Verify(r => r.SaveAsync(), Times.Once);
    }
}
