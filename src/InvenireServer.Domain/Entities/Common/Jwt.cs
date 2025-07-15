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
        var claim = Payload.FirstOrDefault(c => c.Type == "role") ?? throw new NullReferenceException("No 'role' claim is present in the token.");

        return claim.Value;
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
        public const string EMPLOYEE = "EMPLOYEE";

        public const string ADMIN = "ADMIN";
    }
}