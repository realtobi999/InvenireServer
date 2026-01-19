using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Infrastructure.Authentication;

/// <summary>
/// Default implementation of <see cref="IJwtWriter"/>.
/// </summary>
public class JwtWriter : IJwtWriter
{
    public JwtWriter(string key)
    {
        SigningKey = key;
    }

    public string SigningKey { get; }

    /// <summary>
    /// Serializes and signs the provided JWT.
    /// </summary>
    /// <param name="jwt">JWT instance to serialize and sign.</param>
    /// <returns>Signed token string.</returns>
    public string Write(Jwt jwt)
    {
        var headerJson = JsonSerializer.Serialize(jwt.Header.ToDictionary(c => c.Type, object (c) => c.Value));
        var payloadJson = JsonSerializer.Serialize(jwt.Payload.ToDictionary(c => c.Type, object (c) => c.Value));

        var encodedHeader = EncodeBase64Url(Encoding.UTF8.GetBytes(headerJson));
        var encodedPayload = EncodeBase64Url(Encoding.UTF8.GetBytes(payloadJson));

        var signature = ComputeHmacSha256($"{encodedHeader}.{encodedPayload}");

        return $"{encodedHeader}.{encodedPayload}.{signature}";
    }

    /** -------------------------------------------------------------------------- **/

    private string ComputeHmacSha256(string message)
    {
        using var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(SigningKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return EncodeBase64Url(hash);
    }

    private static string EncodeBase64Url(byte[] input)
    {
        return Convert.ToBase64String(input).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}
