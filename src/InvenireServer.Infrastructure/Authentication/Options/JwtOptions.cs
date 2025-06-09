namespace InvenireServer.Infrastructure.Authentication.Options;

public class JwtOptions
{
    public required string Issuer { get; set; }

    public required string SigningKey { get; set; }

    public required int ExpirationTime { get; set; }
}