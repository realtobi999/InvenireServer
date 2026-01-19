using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InvenireServer.Domain.Entities.Common;

/// <summary>
/// Represents a JSON Web Token (JWT).
/// </summary>
public sealed class Jwt
{
    public Jwt(List<Claim> header, List<Claim> payload)
    {
        Header = header;
        Payload = payload;
    }

    public List<Claim> Header { get; }

    public List<Claim> Payload { get; }

    /// <summary>
    /// Gets the role claim from the token.
    /// </summary>
    /// <returns>Role claim value.</returns>
    public string GetRole()
    {
        var claim = Payload.FirstOrDefault(c => c.Type == "role") ?? throw new NullReferenceException("The 'role' claim is present in the token.");

        return claim.Value;
    }

    /// <summary>
    /// Gets the purpose claim from the token.
    /// </summary>
    /// <returns>Purpose claim value, if present.</returns>
    public string? GetPurpose()
    {
        return Payload.FirstOrDefault(c => c.Type == "purpose")?.Value;
    }

    /// <summary>
    /// Gets the expiration time from the token.
    /// </summary>
    /// <returns>Expiration time from the token.</returns>
    public DateTimeOffset? GetExpirationTime()
    {
        var claim = Payload.FirstOrDefault(c => c.Type == "exp") ?? throw new NullReferenceException("The 'exp' claim is present in the token.");

        if (long.TryParse(claim.Value, out var expiration))
        {
            return DateTimeOffset.FromUnixTimeSeconds(expiration);
        }

        throw new FormatException("The 'exp' claim is not a valid Unix timestamp.");
    }

    /// <summary>
    /// Defines JWT policy names.
    /// </summary>
    public static class Policies
    {
        public const string EMPLOYEE = "EMPLOYEE_POLICY";

        public const string UNVERIFIED_EMPLOYEE = "UNVERIFIED_EMPLOYEE_POLICY";

        public const string ADMIN = "ADMIN_POLICY";

        public const string UNVERIFIED_ADMIN = "UNVERIFIED_ADMIN_POLICY";
    }

    /// <summary>
    /// Defines JWT role names.
    /// </summary>
    public static class Roles
    {
        public const string ADMIN = "ADMIN";

        public const string EMPLOYEE = "EMPLOYEE";
    }
}
