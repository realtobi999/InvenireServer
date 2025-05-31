using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InvenireServer.Domain.Core.Dtos.Employees;

/// <summary>
/// Represents the response returned after a successful employee login.
/// </summary>
public record LoginEmployeeResponseDto
{
    /// <summary>
    /// JWT token issued for the authenticated employee.
    /// </summary>
    [Required, JsonPropertyName("token")]
    public required string Token { get; init; }
}
