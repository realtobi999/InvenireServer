namespace InvenireServer.Infrastructure.Authentication.Options;

public class JwtOptions
{
    public required string Issuer { get; init; }
    public required string SigningKey { get; init; }
    public required int ExpirationTime { get; init; }
}