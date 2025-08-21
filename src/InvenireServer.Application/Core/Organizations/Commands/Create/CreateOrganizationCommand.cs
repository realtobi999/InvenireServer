using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Commands.Create;

[JsonRequest]
public record CreateOrganizationCommand : IRequest<CreateOrganizationCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }

    [JsonIgnore]
    public string? FrontendBaseAddress { get; init; }
}