using System.Security.Claims;

namespace InvenireServer.Domain.Entities.Common;

public sealed class Jwt
{
    public Jwt(List<Claim> header, List<Claim> payload)
    {
        Header = header;
        Payload = payload;
    }

    public List<Claim> Header { get; }

    public List<Claim> Payload { get; }

    public string GetRole()
    {
        var claim = Payload.FirstOrDefault(c => c.Type == "role") ?? throw new NullReferenceException("The 'role' claim is present in the token.");

        return claim.Value;
    }

    public string? GetPurpose()
    {
        return Payload.FirstOrDefault(c => c.Type == "purpose")?.Value;
    }

    public DateTimeOffset? GetExpirationTime()
    {
        var claim = Payload.FirstOrDefault(c => c.Type == "exp") ?? throw new NullReferenceException("The 'exp' claim is present in the token.");

        if (long.TryParse(claim.Value, out var expiration))
        {
            return DateTimeOffset.FromUnixTimeSeconds(expiration);
        }

        throw new FormatException("The 'exp' claim is not a valid Unix timestamp.");
    }

    public static class Policies
    {
        public const string EMPLOYEE = "EMPLOYEE_POLICY";

        public const string UNVERIFIED_EMPLOYEE = "UNVERIFIED_EMPLOYEE_POLICY";

        public const string ADMIN = "ADMIN_POLICY";

        public const string UNVERIFIED_ADMIN = "UNVERIFIED_ADMIN_POLICY";
    }

    public static class Roles
    {
        public const string ADMIN = "ADMIN";

        public const string EMPLOYEE = "EMPLOYEE";
    }
}