using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Infrastructure.Authentication.Options;

namespace InvenireServer.Infrastructure.Authentication;

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