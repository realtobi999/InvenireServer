using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Presentation.Extensions;

public static class HttpContextExtensions
{
    public static string ParseJwtToken(this HttpRequest request)
    {
        if (request.Headers.TryParseBearerToken(out var token))
            return token!;

        if (request.Cookies.TryParseTokenFromCookie(out token))
            return token!;

        throw new BadRequest400Exception("The JWT token is missing in the request.");
    }

    private static bool TryParseTokenFromCookie(this IRequestCookieCollection cookies, out string? token)
    {
        token = cookies[Cookie.JWT];
        return !string.IsNullOrEmpty(token);
    }

    private static bool TryParseBearerToken(this IHeaderDictionary headers, out string? token)
    {
        token = null;

        if (!headers.TryGetValue("Authorization", out var value)) return false;

        var header = value.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return false;

        token = header["Bearer ".Length..].Trim();

        if (string.IsNullOrEmpty(token)) return false;

        return true;
    }
}
