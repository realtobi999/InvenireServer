using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Domain.Core.Interfaces.Managers;

/// <summary>
/// Manages access to JWT operations.
/// </summary>
public interface IJwtManager
{
    /// <summary>
    /// Gets the JWT builder responsible for creating tokens.
    /// </summary>
    IJwtBuilder Builder { get; }

    /// <summary>
    /// Gets the JWT writer responsible for token serialization and storage.
    /// </summary>
    IJwtWriter Writer { get; }
}
