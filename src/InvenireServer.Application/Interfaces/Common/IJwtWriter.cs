using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Interfaces.Common;

/// <summary>
/// Defines a writer that serializes and signs JWTs.
/// </summary>
public interface IJwtWriter
{
    /// <summary>
    /// Serializes and signs the provided JWT.
    /// </summary>
    /// <param name="jwt">JWT instance to serialize and sign.</param>
    /// <returns>Signed token string.</returns>
    string Write(Jwt jwt);
}
