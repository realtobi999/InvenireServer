using InvenireServer.Application.Interfaces.Common;

namespace InvenireServer.Application.Interfaces.Managers;

/// <summary>
/// Defines a manager for JWT building and writing.
/// </summary>
public interface IJwtManager
{
    /// <summary>
    /// JWT builder used to create token headers and payloads.
    /// </summary>
    IJwtBuilder Builder { get; }

    /// <summary>
    /// JWT writer used to serialize and sign tokens.
    /// </summary>
    IJwtWriter Writer { get; }
}
