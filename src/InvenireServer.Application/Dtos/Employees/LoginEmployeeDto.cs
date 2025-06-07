using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InvenireServer.Application.Dtos.Employees;

public record LoginEmployeeDto
{
    [Required]
    [JsonPropertyName("email_address")]
    [EmailAddress]
    public required string EmailAddress { get; init; }

    [Required]
    [JsonPropertyName("password")]
    public required string Password { get; init; }
}