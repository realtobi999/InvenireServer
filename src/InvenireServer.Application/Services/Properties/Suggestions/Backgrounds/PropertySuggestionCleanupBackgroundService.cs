using InvenireServer.Application.Interfaces.Common.Transactions;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Interfaces.Services.Properties.Suggestions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvenireServer.Application.Services.Properties.Suggestions.Backgrounds;

public class PropertySuggestionCleanupBackgroundService : BackgroundService, IPropertySuggestionCleanupService
{
    private readonly TimeSpan _interval = TimeSpan.FromDays(1);
    private readonly IServiceScopeFactory _scope;
    private readonly ILogger<PropertySuggestionCleanupBackgroundService> _logger;

    public PropertySuggestionCleanupBackgroundService(IServiceScopeFactory scope, ILogger<PropertySuggestionCleanupBackgroundService> logger)
    {
        _scope = scope;
        _logger = logger;
    }

    public async Task CleanupAsync()
    {
        var services = _scope.CreateScope().ServiceProvider;
        var manager = services.GetRequiredService<IServiceManager>();
        var transaction = services.GetRequiredService<ITransactionScope>();

        var suggestions = await manager.Properties.Suggestion.IndexClosedExpiredAsync();
        if (suggestions.Any())
        {
            await transaction.ExecuteAsync(async () =>
            {
                await manager.Properties.Suggestion.DeleteAsync(suggestions);
            });
        }

        _logger.LogInformation("Closed expired property suggestions cleanup completed at {Time}. Deleted suggestions: {@DeletedSuggestionIds}", DateTime.UtcNow, suggestions.Select(i => i.Id));
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
                _logger.LogError(ex, "An error occurred during property suggestions cleanup.");
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
