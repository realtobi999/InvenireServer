using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InvenireServer.Domain.Core.Dtos.Employees;

/// <summary>
/// Represents the credentials provided by an employee to log in.
/// </summary>
public record LoginEmployeeDto
{
    /// <summary>
    /// Email address associated with the employee's account.
    /// </summary>
    [Required, JsonPropertyName("email_address"), EmailAddress]
    public required string EmailAddress { get; init; }

    /// <summary>
    /// Password used to authenticate the employee's account.
    /// </summary>
    [Required, JsonPropertyName("password")]
    public required string Password { get; init; }
}
