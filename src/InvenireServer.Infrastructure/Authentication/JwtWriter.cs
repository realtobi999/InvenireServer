using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Infrastructure.Authentication;

/// <summary>
/// Signs and serializes a JWT using a symmetric key.
/// </summary>
public class JwtWriter : IJwtWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JwtWriter"/> class with the specified signing key.
    /// </summary>
    /// <param name="key">The key used for signing the JWT.</param>
    public JwtWriter(string key)
    {
        SigningKey = key;
    }

    /// <summary>
    /// The key used to sign the JWT.
    /// </summary>
    public string SigningKey { get; set; }

    /// <summary>
    /// Serializes and signs the given JWT using HMAC-SHA256.
    /// </summary>
    /// <param name="jwt">The JWT object containing header and payload claims.</param>
    /// <returns>A compact JWT string including the signature.</returns>
    public string Write(Jwt jwt)
    {
        var headerJson = JsonSerializer.Serialize(jwt.Header.ToDictionary(c => c.Type, c => (object)c.Value));
        var payloadJson = JsonSerializer.Serialize(jwt.Payload.ToDictionary(c => c.Type, c => (object)c.Value));

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

    private static string EncodeBase64Url(byte[] input) =>
        Convert.ToBase64String(input).Replace("+", "-").Replace("/", "_").Replace("=", "");
}