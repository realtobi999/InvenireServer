namespace InvenireServer.Application.Interfaces.Services.Properties.Suggestions;

/// <summary>
/// Defines a cleanup service for closed and expired property suggestions.
/// </summary>
public interface IPropertySuggestionCleanupService
{
    /// <summary>
    /// Performs cleanup of closed and expired property suggestions.
    /// </summary>
    /// <returns>Awaitable task representing the cleanup operation.</returns>
    Task CleanupAsync();
}
