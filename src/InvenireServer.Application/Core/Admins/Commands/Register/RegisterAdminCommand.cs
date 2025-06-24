using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Admins.Commands.Register;

public record RegisterAdminCommand : IRequest<RegisterAdminCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [Required]
    [JsonPropertyName("name")]
    [MaxLength(Admin.MAX_NAME_LENGTH)]
    public required string Name { get; init; }

    [Required]
    [EmailAddress]
    [JsonPropertyName("email_address")]
    [MaxLength(Employee.MAX_EMAIL_ADDRESS_LENGTH)]
    public required string EmailAddress { get; init; }

    [Required]
    [JsonPropertyName("password")]
    [MinLength(Employee.MIN_PASSWORD_LENGTH)]
    [MaxLength(Employee.MAX_PASSWORD_LENGTH)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$")]
    public required string Password { get; init; }

    [Required]
    [JsonPropertyName("password_confirm")]
    [SameAs(nameof(Password))]
    public required string PasswordConfirm { get; init; }
}