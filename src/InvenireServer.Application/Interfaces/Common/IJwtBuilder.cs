using System.Security.Claims;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Interfaces.Common;

/// <summary>
/// Defines the contract for building JWT tokens from claims.
/// </summary>
public interface IJwtBuilder
{
    /// <summary>
    /// Builds a JWT token from the provided claims.
    /// </summary>
    /// <param name="claims">The list of claims to include in the token.</param>
    /// <returns>The constructed JWT token.</returns>
    Jwt Build(List<Claim> claims);
}
