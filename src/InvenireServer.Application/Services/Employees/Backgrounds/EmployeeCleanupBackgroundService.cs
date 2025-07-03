using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Interfaces.Services.Employees;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvenireServer.Application.Services.Employees.Backgrounds;

public class EmployeeCleanupBackgroundService : BackgroundService, IEmployeeCleanupService
{
    private readonly TimeSpan _interval = TimeSpan.FromDays(7);
    private readonly ILogger<EmployeeCleanupBackgroundService> _logger;
    private readonly IServiceScopeFactory _scope;

    public EmployeeCleanupBackgroundService(IServiceScopeFactory scope, ILogger<EmployeeCleanupBackgroundService> logger)
    {
        _scope = scope;
        _logger = logger;
    }

    public async Task CleanupAsync()
    {
        var manager = _scope.CreateScope().ServiceProvider.GetRequiredService<IRepositoryManager>();

        foreach (var employee in await manager.Employees.IndexInactiveAsync()) manager.Employees.Delete(employee);

        await manager.SaveAsync();

        _logger.LogInformation("Unverified employees successful completed at {Time}", DateTime.UtcNow);
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during employee cleanup.");
            }

            try
            {
                await Task.Delay(_interval, token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}