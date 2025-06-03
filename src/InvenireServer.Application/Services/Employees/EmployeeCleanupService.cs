using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvenireServer.Application.Services.Employees;

/// <summary>
/// Background service that periodically removes unverified employee accounts older than a defined threshold.
/// </summary>
public class EmployeeCleanupService : BackgroundService, IEmployeeCleanupService
{
    private readonly TimeSpan _interval = TimeSpan.FromDays(7);
    private readonly IServiceScopeFactory _scope;
    private readonly ILogger<EmployeeCleanupService> _logger;


    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeCleanupService"/> class.
    /// </summary>
    /// <param name="scope">Factory for creating service scopes.</param>
    /// <param name="logger">Logger used to log cleanup activity and errors.</param>
    public EmployeeCleanupService(IServiceScopeFactory scope, ILogger<EmployeeCleanupService> logger)
    {
        _scope = scope;
        _logger = logger;
    }

    /// <summary>
    /// Performs cleanup by removing unverified employee accounts older than 7 days.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CleanupAsync()
    {
        var manager = _scope.CreateScope().ServiceProvider.GetRequiredService<IRepositoryManager>();

        foreach (var employee in await manager.Employees.IndexAsync(e => !e.IsVerified && e.CreatedAt <= DateTimeOffset.UtcNow.AddDays(-7)))
        {
            manager.Employees.Delete(employee);
        }

        await manager.SaveAsync();

        _logger.LogInformation("Unverified users cleanup completed at {Time}", DateTime.UtcNow);
    }

    /// <inheritdoc/>
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
