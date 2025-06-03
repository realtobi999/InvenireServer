using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Presentation.Extensions;

/// <summary>
/// Provides extension methods for working with HTTP context headers.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Extracts the Bearer token from the Authorization header.
    /// </summary>
    /// <param name="headers">The collection of HTTP request headers.</param>
    /// <returns>The Bearer token string if found and valid.</returns>
    /// <exception cref="BadRequest400Exception">
    /// Thrown when the Authorization header is missing, not in the expected format, or the token part is empty or missing.
    /// </exception>
    public static string ParseBearerToken(this IHeaderDictionary headers)
    {
        var header = headers.Authorization.ToString();
        if (header is null)
        {
            throw new BadRequest400Exception("Authorization header is missing. Expected Format: BEARER <TOKEN>");
        }

        const string prefix = "Bearer ";
        if (!header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequest400Exception("Invalid authorization header format. Expected format: BEARER <TOKEN>");
        }

        var token = header[prefix.Length..].Trim();

        if (string.IsNullOrEmpty(token))
        {
            throw new BadRequest400Exception("Token is missing in the authorization header.");
        }

        return token;
    }
}