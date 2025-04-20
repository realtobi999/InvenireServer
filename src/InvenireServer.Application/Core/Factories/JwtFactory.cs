using System.Text;
using System.Text.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using InvenireServer.Domain.Core.Entities.Common;
using InvenireServer.Domain.Core.Interfaces.Factories;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Application.Core.Factories;

public class JwtFactory : IJwtFactory
{
    public string Issuer { get; set; }
    public string SigningKey { get; set; }
    public TimeSpan ExpirationTime { get; set; }

    private const int DefaultExpirationTime = 30;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtFactory"/> class with the specified key, issuer, and optional expiration time.
    /// </summary>
    /// <param name="key">The signing key used to sign the JWT token.</param>
    /// <param name="issuer">The issuer of the JWT token.</param>
    /// <param name="expiration">The expiration time in minutes (default is 30 minutes).</param>
    public JwtFactory(string key, string issuer, int expiration = DefaultExpirationTime)
    {
        Issuer = issuer;
        SigningKey = key;
        ExpirationTime = TimeSpan.FromMinutes(expiration);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtFactory"/> class using configuration values.
    /// </summary>
    /// <param name="configuration">The configuration containing JWT settings (Issuer, Key, ExpirationTime).</param>
    /// <exception cref="NullReferenceException">Thrown when required configuration values are missing.</exception>
    /// <exception cref="ArgumentException">Thrown when the expiration time configuration is invalid.</exception>
    public JwtFactory(IConfiguration configuration)
    {
        Issuer = configuration.GetValue<string>("Jwt:Issuer") ?? throw new NullReferenceException("JWT Issuer configuration is missing");
        SigningKey = configuration.GetValue<string>("Jwt:Key") ?? throw new NullReferenceException("JWT Key configuration is missing");

        var expiration = configuration.GetValue<string>("Jwt:ExpirationTime");
        if (string.IsNullOrWhiteSpace(expiration))
        {
            ExpirationTime = TimeSpan.FromMinutes(DefaultExpirationTime);
        }

        if (!int.TryParse(expiration, out var minutes))
        {
            throw new ArgumentException("JWT ExpirationTime configuration is invalid. Ensure that the value is a valid integer.");
        }

        ExpirationTime = TimeSpan.FromMinutes(minutes);
    }

    public Jwt Create(IEnumerable<Claim> claims)
    {
        var header = new List<Claim>
        {
            new("alg", "HS256"),
            new("typ", "JWT")
        };
        var payload = CreateValidPayload([.. claims]);

        // Compute the signature.
        var headerJson = JsonSerializer.Serialize(header.ToDictionary(c => c.Type, object (c) => c.Value));
        var payloadJson = JsonSerializer.Serialize(payload.ToDictionary(c => c.Type, object (c) => c.Value));
        var signature = ComputeHmacSha256($"{EncodeBase64Url(Encoding.ASCII.GetBytes(headerJson))}.{EncodeBase64Url(Encoding.ASCII.GetBytes(payloadJson))}");

        return new Jwt(header, [.. payload], signature);
    }

    /// <summary>
    /// Ensures the validity of the claims by adding missing standard claims such as "iss", "aud", and "exp".
    /// </summary>
    /// <param name="claims">The claims to validate and potentially add to.</param>
    /// <returns>A list of claims including any missing standard claims.</returns>
    private List<Claim> CreateValidPayload(List<Claim> claims)
    {
        if (claims.All(c => c.Type != "iss"))
        {
            claims.Add(new Claim("iss", Issuer));
        }
        if (claims.All(c => c.Type != "aud"))
        {
            claims.Add(new Claim("aud", Issuer));
        }
        if (claims.All(c => c.Type != "exp"))
        {
            claims.Add(new Claim("exp", DateTimeOffset.UtcNow.Add(ExpirationTime).ToUnixTimeSeconds().ToString()));
        }

        return claims;
    }

    /// <summary>
    /// Computes an HMAC SHA-256 signature for a given message using the signing key.
    /// </summary>
    /// <param name="message">The message to be signed.</param>
    /// <returns>The computed signature as a base64-encoded string.</returns>
    private string ComputeHmacSha256(string message)
    {
        using var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(SigningKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return EncodeBase64Url(hash);
    }

    /// <summary>
    /// Encodes a byte array to a URL-safe base64 string.
    /// </summary>
    /// <param name="bytes">The byte array to encode.</param>
    /// <returns>The URL-safe base64-encoded string.</returns>
    private static string EncodeBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
