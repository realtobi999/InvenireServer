using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Infrastructure.Authentication.Options;

namespace InvenireServer.Infrastructure.Authentication;

/// <summary>
/// Default implementation of <see cref="IJwtManager"/>.
/// </summary>
public class JwtManager : IJwtManager
{
    public JwtManager(JwtOptions options)
    {
        Builder = new JwtBuilder(options.Issuer, options.ExpirationTime);
        Writer = new JwtWriter(options.SigningKey);
    }

    public IJwtBuilder Builder { get; }

    public IJwtWriter Writer { get; }
}
