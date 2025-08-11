using InvenireServer.Application.Interfaces.Common.Transactions;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Interfaces.Services.Organizations.Invitations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvenireServer.Application.Services.Organizations.Invitations.Backgrounds;

public class OrganizationInvitationCleanupBackgroundService : BackgroundService, IOrganizationInvitationCleanupService
{
    private readonly TimeSpan _interval = TimeSpan.FromDays(1);
    private readonly IServiceScopeFactory _scope;
    private readonly ILogger<OrganizationInvitationCleanupBackgroundService> _logger;

    public OrganizationInvitationCleanupBackgroundService(IServiceScopeFactory scope, ILogger<OrganizationInvitationCleanupBackgroundService> logger)
    {
        _scope = scope;
        _logger = logger;
    }

    public async Task CleanupAsync()
    {
        var services = _scope.CreateScope().ServiceProvider;
        var manager = services.GetRequiredService<IRepositoryManager>();
        var transaction = services.GetRequiredService<ITransactionScope>();

        var invitations = await manager.Organizations.Invitations.IndexExpiredAsync();
        if (invitations.Any())
        {
            await transaction.ExecuteAsync(async () =>
            {
                foreach (var invitation in invitations) manager.Organizations.Invitations.Delete(invitation);
                await manager.SaveOrThrowAsync();
            });
        }

        _logger.LogInformation("Expired organizations invitations cleanup completed at {Time}. Deleted invitations: {@DeletedInvitationIds}", DateTime.UtcNow, invitations.Select(i => i.Id));
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
                _logger.LogError(ex, "An error occurred during organization invitations cleanup.");
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
