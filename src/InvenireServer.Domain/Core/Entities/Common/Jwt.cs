using System.Text;
using System.Text.Json;
using System.Security.Claims;

namespace InvenireServer.Domain.Core.Entities.Common;

/// <summary>
/// Represents a JSON Web Token (JWT).
/// </summary>
public sealed class Jwt
{
    /// <summary>
    /// Gets or sets the header claims of the JWT.
    /// </summary>
    public List<Claim> Header { get; }

    /// <summary>
    /// Gets or sets the payload claims of the JWT.
    /// </summary>
    public List<Claim> Payload { get; }

    /// <summary>
    /// Gets or sets the signature of the JWT.
    /// </summary>
    public string Signature { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Jwt"/> class with the specified header, payload, and signature.
    /// </summary>
    /// <param name="header">The header claims of the JWT.</param>
    /// <param name="payload">The payload claims of the JWT.</param>
    /// <param name="signature">The signature of the JWT.</param>
    public Jwt(List<Claim> header, List<Claim> payload, string signature)
    {
        Header = header;
        Payload = payload;
        Signature = signature;
    }

    /// <summary>
    /// Converts the JWT to its string representation.
    /// </summary>
    /// <returns>The JWT as a string in the format: header.payload.signature.</returns>
    public string Write()
    {
        // Convert claims to dictionaries and serialize to Json.
        var headerJson = JsonSerializer.Serialize(Header.ToDictionary(c => c.Type, object (c) => c.Value));
        var payloadJson = JsonSerializer.Serialize(Payload.ToDictionary(c => c.Type, object (c) => c.Value));

        // Base64URL encode.
        var encodedHeader = EncodeBase64Url(Encoding.UTF8.GetBytes(headerJson));
        var encodedPayload = EncodeBase64Url(Encoding.UTF8.GetBytes(payloadJson));

        return $"{encodedHeader}.{encodedPayload}.{Signature}";
    }

    /// <summary>
    /// Parses a JWT string and returns the corresponding <see cref="Jwt"/> object.
    /// </summary>
    /// <param name="token">The JWT string to parse.</param>
    /// <returns>The parsed <see cref="Jwt"/> object.</returns>
    /// <exception cref="ArgumentException">Thrown when the token format is invalid.</exception>
    public static Jwt Parse(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            throw new ArgumentException("Invalid JWT token format.");

        // Decode and parse header.
        var header = ParseJsonToClaims(DecodeBase64Url(parts[0]));

        // Decode and parse payload
        var payload = ParseJsonToClaims(DecodeBase64Url(parts[1]));

        // Signature is just raw base64url.
        var signature = parts[2];

        return new Jwt(header, payload, signature);
    }

    /// <summary>
    /// Decodes a Base64URL-encoded string to a UTF-8 string.
    /// </summary>
    /// <param name="input">The Base64URL-encoded string to decode.</param>
    /// <returns>The decoded string.</returns>
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

    /// <summary>
    /// Encodes a byte array to a URL-safe Base64 string.
    /// </summary>
    /// <param name="input">The byte array to encode.</param>
    /// <returns>The URL-safe Base64-encoded string.</returns>
    private static string EncodeBase64Url(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    /// <summary>
    /// Parses a JSON string into a list of claims.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>A list of <see cref="Claim"/> objects.</returns>
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
            var value = kvp.Value.ValueKind switch
            {
                JsonValueKind.String => kvp.Value.GetString() ?? "",
                JsonValueKind.Number => kvp.Value.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => kvp.Value.ToString()
            };
            claims.Add(new Claim(kvp.Key, value));
        }

        return claims;
    }
}