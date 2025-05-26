using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InvenireServer.Domain.Core.Dtos.Employees;

public record LoginEmployeeResponseDto
{
    [Required, JsonPropertyName("token")]
    public required string Token { get; init; }
}
