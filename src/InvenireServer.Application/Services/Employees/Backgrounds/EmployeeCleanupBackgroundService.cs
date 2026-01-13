using InvenireServer.Application.Interfaces.Common.Transactions;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Interfaces.Services.Employees;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvenireServer.Application.Services.Employees.Backgrounds;

/// <summary>
/// Background service that performs periodic cleanup of inactive employees.
/// </summary>
public class EmployeeCleanupBackgroundService : BackgroundService, IEmployeeCleanupService
{
    private readonly TimeSpan _interval = TimeSpan.FromDays(1);
    private readonly IServiceScopeFactory _scope;
    private readonly ILogger<EmployeeCleanupBackgroundService> _logger;

    public EmployeeCleanupBackgroundService(IServiceScopeFactory scope, ILogger<EmployeeCleanupBackgroundService> logger)
    {
        _scope = scope;
        _logger = logger;
    }

    /// <summary>
    /// Performs cleanup of inactive employees.
    /// </summary>
    /// <returns>Awaitable task representing the cleanup operation.</returns>
    public async Task CleanupAsync()
    {
        var services = _scope.CreateScope().ServiceProvider;
        var manager = services.GetRequiredService<IRepositoryManager>();
        var transaction = services.GetRequiredService<ITransactionScope>();

        var employees = await manager.Employees.IndexInactiveAsync();
        if (employees.Any())
        {
            await transaction.ExecuteAsync(async () =>
            {
                foreach (var employee in employees) manager.Employees.Delete(employee);
                await manager.SaveOrThrowAsync();
            });
        }

        _logger.LogInformation("Unverified employees cleanup successfully completed at {Time}. Deleted employees: {@DeletedEmployeeIds}", DateTime.UtcNow, employees.Select(a => a.Id));
    }

    /// <summary>
    /// Runs the background execution loop.
    /// </summary>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Awaitable task representing the background execution.</returns>
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
