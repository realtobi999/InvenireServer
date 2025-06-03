using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Interfaces.Common;

/// <summary>
/// Defines the contract for writing or serializing JWT tokens.
/// </summary>
public interface IJwtWriter
{
    /// <summary>
    /// Serializes the given JWT into a string representation.
    /// </summary>
    /// <param name="jwt">The JWT to serialize.</param>
    /// <returns>The serialized JWT as a string.</returns>
    string Write(Jwt jwt);
}