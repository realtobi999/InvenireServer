using InvenireServer.Domain.Core.Options;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Managers;

namespace InvenireServer.Application.Core.Authentication;

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
