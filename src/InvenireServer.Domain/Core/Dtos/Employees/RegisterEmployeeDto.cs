using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InvenireServer.Domain.Core.Attributes;
using InvenireServer.Domain.Core.Entities;

namespace InvenireServer.Domain.Core.Dtos.Employees;

public record RegisterEmployeeDto
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [Required, JsonPropertyName("name"), MaxLength(Employee.MAX_NAME_LENGTH)]
    public required string Name { get; set; }

    [Required, JsonPropertyName("email_address"), MaxLength(Employee.MAX_EMAIL_ADDRESS_LENGTH), EmailAddress]
    public required string EmailAddress { get; set; }

    [Required, JsonPropertyName("password"), MinLength(Employee.MIN_PASSWORD_LENGTH), MaxLength(Employee.MAX_PASSWORD_LENGTH), RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$")]
    public required string Password { get; set; }

    [Required, JsonPropertyName("password_confirm"), SameAs(nameof(Password))]
    public required string PasswordConfirm { get; set; }
}
