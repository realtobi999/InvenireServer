using InvenireServer.Application.Attributes;

namespace InvenireServer.Application.Dtos.Admins;

[JsonResponse]
public record AdminDto
{
    public Guid Id { get; init; }
    public Guid? OrganizationId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? FullName { get; init; }
    public string? EmailAddress { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? LastUpdatedAt { get; init; }
}
