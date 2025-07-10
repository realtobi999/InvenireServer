namespace InvenireServer.Application.Dtos.Admins.Email;

public record AdminOrganizationCreationEmailDto
{
    public required string AdminAddress { get; init; }

    public required string AdminName { get; init; }

    public required string OrganizationName { get; init; }

    public required string DashboardLink { get; init; }
}