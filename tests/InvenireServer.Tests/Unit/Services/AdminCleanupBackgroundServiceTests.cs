using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Services.Admins.Backgrounds;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Integration.Fakers.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvenireServer.Tests.Unit.Services;

public class AdminCleanupBackgroundServiceTests
{
    private readonly Mock<ILogger<AdminCleanupBackgroundService>> _mockLogger = new();
    private readonly Mock<IRepositoryManager> _mockRepositoryManager = new();
    private readonly Mock<IServiceScope> _mockServiceScope = new();
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory = new();

    private AdminCleanupBackgroundService CreateService()
    {
        _mockServiceScope.Setup(ss => ss.ServiceProvider.GetService(typeof(IRepositoryManager))).Returns(_mockRepositoryManager.Object);
        _mockServiceScopeFactory.Setup(ssf => ssf.CreateScope()).Returns(_mockServiceScope.Object);

        return new AdminCleanupBackgroundService(_mockServiceScopeFactory.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CleanupAsync_RemovesUnverifiedOldEmployees()
    {
        // Prepare.
        var now = DateTimeOffset.UtcNow;
        var admins = new List<Admin>();

        // Create admins that are categorized for deletion.
        for (var i = 0; i < 2; i++)
        {
            var employee = new AdminFaker().Generate();

            employee.IsVerified = false;
            employee.CreatedAt = now.AddDays(-8);

            admins.Add(employee);
        }

        _mockRepositoryManager
            .Setup(repo => repo.Admins.IndexInactiveAsync())
            .ReturnsAsync(admins);

        _mockRepositoryManager
            .Setup(m => m.SaveAsync())
            .ReturnsAsync(1);

        var service = CreateService();

        // Act & Assert.
        await service.CleanupAsync();

        foreach (var admin in admins) _mockRepositoryManager.Verify(repo => repo.Admins.Delete(admin), Times.Once);

        _mockRepositoryManager.Verify(r => r.SaveAsync(), Times.Once);
    }
}