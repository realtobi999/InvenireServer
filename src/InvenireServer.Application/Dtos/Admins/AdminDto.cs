using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Dtos.Admins;

[JsonResponse]
public record AdminDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("organization_id")]
    public required Guid? OrganizationId { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; init; }

    public static AdminDto FromAdmin(Admin admin)
    {
        return new AdminDto
        {
            Id = admin.Id,
            OrganizationId = admin.OrganizationId,
            Name = admin.Name,
            EmailAddress = admin.EmailAddress,
            CreatedAt = admin.CreatedAt,
            LastUpdatedAt = admin.LastUpdatedAt,
        };
    }
}
