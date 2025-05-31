using System.Security.Claims;
using InvenireServer.Domain.Core.Entities.Common;

namespace InvenireServer.Domain.Core.Interfaces.Factories;

/// <summary>
/// Defines a contract for creating JWT tokens with configurable issuer, signing key, and expiration.
/// </summary>
public interface IJwtFactory
{
    /// <summary>
    /// Gets or sets the issuer of the JWT token.
    /// </summary>
    string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the signing key used to sign the JWT token.
    /// </summary>
    string SigningKey { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for the JWT token.
    /// </summary>
    TimeSpan ExpirationTime { get; set; }

    /// <summary>
    /// Creates a JWT token from the specified claims.
    /// </summary>
    /// <param name="payload">The claims to include in the JWT token.</param>
    /// <returns>A <see cref="Jwt"/> object representing the created JWT token.</returns>
    Jwt Create(IEnumerable<Claim> payload);
}