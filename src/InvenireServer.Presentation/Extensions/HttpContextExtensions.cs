using InvenireServer.Domain.Core.Exceptions.Http;

namespace InvenireServer.Presentation.Extensions;

public static class HttpContextExtensions
{
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
