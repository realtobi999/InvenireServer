using System.Security.Claims;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Interfaces.Common;

/// <summary>
/// Defines a JWT builder.
/// </summary>
public interface IJwtBuilder
{
    /// <summary>
    /// Builds a JWT using the provided claims.
    /// </summary>
    /// <param name="claims">Claims to include in the token payload.</param>
    /// <returns>The constructed <see cref="Jwt"/>.</returns>
    Jwt Build(List<Claim> claims);
}
