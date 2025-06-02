using System.Security.Claims;

namespace InvenireServer.Domain.Core.Entities.Common;

/// <summary>
/// Represents a JSON Web Token (JWT) containing header and payload claims.
/// </summary>
public sealed class Jwt
{
    /// <summary>
    /// Header claims included in the JWT, typically specifying the algorithm and token type.
    /// </summary>
    public List<Claim> Header { get; }

    /// <summary>
    /// Payload claims included in the JWT, representing the token's subject and associated metadata.
    /// </summary>
    public List<Claim> Payload { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Jwt"/> class using the specified header and payload claims.
    /// </summary>
    /// <param name="header">JWT header claims, such as algorithm and token type.</param>
    /// <param name="payload">JWT payload claims, such as subject identifiers and roles.</param>
    public Jwt(List<Claim> header, List<Claim> payload)
    {
        Header = header;
        Payload = payload;
    }

    /// <summary>
    /// Defines authorization policy names.
    /// </summary>
    public static class Policies
    {
        /// <summary>
        /// Policy allowing access to verified employees.
        /// </summary>
        public const string EMPLOYEE = "EMPLOYEE_POLICY";

        /// <summary>
        /// Policy allowing access to employees who have not yet verified their email.
        /// </summary>
        public const string UNVERIFIED_EMPLOYEE = "UNVERIFIED_EMPLOYEE_POLICY";
    }

    /// <summary>
    /// Defines role identifiers.
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// Role assigned to authenticated employees.
        /// </summary>
        public const string EMPLOYEE = "EMPLOYEE";
    }
}
