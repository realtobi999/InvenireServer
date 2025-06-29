using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Interfaces.Services.Admins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvenireServer.Application.Services.Admins.Backgrounds;

public class AdminCleanupBackgroundService : BackgroundService, IAdminCleanupService
{
    private readonly TimeSpan _interval = TimeSpan.FromDays(7);
    private readonly ILogger<AdminCleanupBackgroundService> _logger;
    private readonly IServiceScopeFactory _scope;

    public AdminCleanupBackgroundService(IServiceScopeFactory scope, ILogger<AdminCleanupBackgroundService> logger)
    {
        _scope = scope;
        _logger = logger;
    }

    public async Task CleanupAsync()
    {
        var manager = _scope.CreateScope().ServiceProvider.GetRequiredService<IRepositoryManager>();

        foreach (var admin in await manager.Admins.IndexInactiveAsync()) manager.Admins.Delete(admin);

        await manager.SaveAsync();

        _logger.LogInformation("Unverified admins cleanup completed at {Time}", DateTime.UtcNow);
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