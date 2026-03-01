using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

/// <summary>
/// Represents a request to create an organization invitation.
/// </summary>
[JsonRequest]
public record CreateOrganizationInvitationCommand : IRequest<CreateOrganizationInvitationCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("employee_id")]
    public Guid? EmployeeId { get; init; }

    [JsonPropertyName("employee_email_address")]
    public string? EmployeeEmailAddress { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}

/// <summary>
/// Represents a CSV row for creating an organization invitation.
/// </summary>
public record CreateOrganizationInvitationCommandCsvRow
{
    [Name("id")]
    [Optional]
    public Guid? Id { get; init; }

    [Name("description")]
    public string? Description { get; init; }

    [Name("employee_id")]
    [Optional]
    public Guid? EmployeeId { get; init; }

    [Name("employee_email_address")]
    public string? EmployeeEmailAddress { get; set; }
}
