using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InvenireServer.Domain.Core.Dtos.Employees;

public record LoginEmployeeDto
{

    [Required, JsonPropertyName("email_address"), EmailAddress]
    public required string EmailAddress { get; set; }

    [Required, JsonPropertyName("password")]
    public required string Password { get; set; }
}
