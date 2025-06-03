using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Infrastructure.Authentication.Options;

namespace InvenireServer.Infrastructure.Authentication;

/// <summary>
/// Provides access to JWT building and writing functionality.
/// </summary>
public class JwtManager : IJwtManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JwtManager"/> class using the specified JWT options.
    /// </summary>
    /// <param name="options">The JWT options containing issuer, expiration time, and signing key.</param>
    public JwtManager(JwtOptions options)
    {
        Builder = new JwtBuilder(options.Issuer, options.ExpirationTime);
        Writer = new JwtWriter(options.SigningKey);
    }

    /// <inheritdoc/>
    public IJwtBuilder Builder { get; }

    /// <inheritdoc/>
    public IJwtWriter Writer { get; }
}