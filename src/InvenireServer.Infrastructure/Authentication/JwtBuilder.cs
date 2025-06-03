using System.Security.Claims;
using System.Text;
using System.Text.Json;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Infrastructure.Authentication;

/// <summary>
/// Constructs and parses JWT's with configurable issuer and expiration.
/// </summary>
public class JwtBuilder : IJwtBuilder
{
    private const int DEFAULT_EXPIRATION_TIME = 30;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtBuilder"/> class.
    /// </summary>
    /// <param name="issuer">The issuer claim value to include in the token.</param>
    /// <param name="expiration">The expiration time in minutes. Defaults to 30 minutes.</param>
    public JwtBuilder(string issuer, int expiration = DEFAULT_EXPIRATION_TIME)
    {
        Issuer = issuer;
        ExpirationTime = TimeSpan.FromMinutes(expiration);
    }

    /// <summary>
    /// The value assigned to the 'iss' (issuer) claim in the token.
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// The time span for which the token is valid after issuance.
    /// </summary>
    public TimeSpan ExpirationTime { get; set; }

    /// <summary>
    /// Builds a new JWT token with the specified claims, issuer, and expiration time.
    /// </summary>
    /// <param name="claims">Optional list of claims to include in the payload.</param>
    /// <returns>The constructed <see cref="Jwt"/> token.</returns>
    public Jwt Build(List<Claim>? claims = null)
    {
        var header = new List<Claim>
        {
            new("alg", "HS256"),
            new("typ", "JWT")
        };

        var payload = claims ?? [];

        if (payload.All(c => c.Type != "iss"))
        {
            payload.Add(new Claim("iss", Issuer));
        }

        if (payload.All(c => c.Type != "aud"))
        {
            payload.Add(new Claim("aud", Issuer));
        }

        if (payload.All(c => c.Type != "exp"))
        {
            payload.Add(new Claim("exp", DateTimeOffset.UtcNow.Add(ExpirationTime).ToUnixTimeSeconds().ToString()));
        }

        return new Jwt(header, payload);
    }

    /// <summary>
    /// Parses a serialized JWT string into a structured <see cref="Jwt"/> object.
    /// </summary>
    /// <param name="token">The JWT string to parse.</param>
    /// <returns>The parsed JWT containing its header and payload claims.</returns>
    /// <exception cref="ArgumentException">Thrown if the token format is invalid.</exception>
    public static Jwt Parse(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            throw new ArgumentException("Invalid JWT token format.");

        var header = ParseJsonToClaims(DecodeBase64Url(parts[0]));
        var payload = ParseJsonToClaims(DecodeBase64Url(parts[1]));

        return new Jwt(header, payload);
    }

    /** -------------------------------------------------------------------------- **/
    private static List<Claim> ParseJsonToClaims(string json)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        if (dict == null)
        {
            return [];
        }

        var claims = new List<Claim>();
        foreach (var kvp in dict)
        {
            claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
        }

        return claims;
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