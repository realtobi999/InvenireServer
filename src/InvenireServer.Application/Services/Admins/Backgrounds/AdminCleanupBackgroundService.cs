using InvenireServer.Application.Interfaces.Common.Transactions;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Interfaces.Services.Admins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvenireServer.Application.Services.Admins.Backgrounds;

public class AdminCleanupBackgroundService : BackgroundService, IAdminCleanupService
{
    private readonly TimeSpan _interval = TimeSpan.FromDays(1);
    private readonly ILogger<AdminCleanupBackgroundService> _logger;
    private readonly IServiceScopeFactory _scope;

    public AdminCleanupBackgroundService(IServiceScopeFactory scope, ILogger<AdminCleanupBackgroundService> logger)
    {
        _scope = scope;
        _logger = logger;
    }

    public async Task CleanupAsync()
    {
        var services = _scope.CreateScope().ServiceProvider;
        var manager = services.GetRequiredService<IRepositoryManager>();
        var transaction = services.GetRequiredService<ITransactionScope>();

        var admins = await manager.Admins.IndexInactiveAsync();
        if (admins.Any())
        {
            await transaction.ExecuteAsync(async () =>
            {
                foreach (var admin in admins) manager.Admins.Delete(admin);
                await manager.SaveOrThrowAsync();
            });
        }

        _logger.LogInformation("Unverified admins cleanup successfully completed at {Time}. Deleted admins: {@DeletedAdminIds}", DateTime.UtcNow, admins.Select(a => a.Id));

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