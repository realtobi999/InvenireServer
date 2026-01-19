using System.Security.Claims;
using System.Text;
using System.Text.Json;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Infrastructure.Authentication;

/// <summary>
/// Default implementation of <see cref="IJwtBuilder"/>.
/// </summary>
public class JwtBuilder : IJwtBuilder
{
    private const int DEFAULT_EXPIRATION_TIME = 30;

    public JwtBuilder(string issuer, int expiration = DEFAULT_EXPIRATION_TIME)
    {
        Issuer = issuer;
        ExpirationTime = TimeSpan.FromMinutes(expiration);
    }

    public string Issuer { get; }

    public TimeSpan ExpirationTime { get; }

    /// <summary>
    /// Builds a JWT using the provided claims.
    /// </summary>
    /// <param name="claims">Claims to include in the token payload.</param>
    /// <returns>The constructed <see cref="Jwt"/>.</returns>
    public Jwt Build(List<Claim>? claims = null)
    {
        var header = new List<Claim>
        {
            new("alg", "HS256"),
            new("typ", "JWT")
        };

        var payload = claims ?? [];

        if (payload.All(c => c.Type != "iss")) payload.Add(new Claim("iss", Issuer));

        if (payload.All(c => c.Type != "aud")) payload.Add(new Claim("aud", Issuer));

        if (payload.All(c => c.Type != "exp")) payload.Add(new Claim("exp", DateTimeOffset.UtcNow.Add(ExpirationTime).ToUnixTimeSeconds().ToString()));

        return new Jwt(header, payload);
    }

    /// <summary>
    /// Parses a JWT string into a <see cref="Jwt"/>.
    /// </summary>
    /// <param name="token">Token string to parse.</param>
    /// <returns>Parsed JWT.</returns>
    public static Jwt Parse(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3) throw new ArgumentException("Invalid JWT token format.");

        var header = ParseJsonToClaims(DecodeBase64Url(parts[0]));
        var payload = ParseJsonToClaims(DecodeBase64Url(parts[1]));

        return new Jwt(header, payload);
    }

    private static List<Claim> ParseJsonToClaims(string json)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        return dict == null ? [] : dict.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())).ToList();
    }

    private static string DecodeBase64Url(string input)
    {
        input = input.Replace('-', '+').Replace('_', '/');
        switch (input.Length % 4)
        {
            case 2: input += "=="; break;
            case 3: input += "="; break;
            case 0: break;
            default: throw new FormatException("Invalid Base64URL string.");
        }

        var bytes = Convert.FromBase64String(input);
        return Encoding.UTF8.GetString(bytes);
    }
}
