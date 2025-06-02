namespace InvenireServer.Domain.Core.Options;

/// <summary>
/// Represents configuration options for JWT token generation.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Gets or sets the token issuer identifier.
    /// </summary>
    public required string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the signing key used to sign the token.
    /// </summary>
    public required string SigningKey { get; set; }

    /// <summary>
    /// Gets or sets the token expiration time in minutes.
    /// </summary>
    public required int ExpirationTime { get; set; }
}
