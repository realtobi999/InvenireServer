using System.Text.Json.Serialization;

namespace InvenireServer.Application.Core.Employees.Commands.Recover.Send;

public record SendPasswordRecoveryEmployeeCommand : IRequest
{
    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonIgnore]
    public string? FrontendBaseAddress { get; init; }
}
