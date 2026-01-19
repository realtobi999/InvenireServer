using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Tests.Fakers.Organizations;

/// <summary>
/// Provides fake <see cref="OrganizationInvitation"/> instances for tests.
/// </summary>
public sealed class OrganizationInvitationFaker : Faker<OrganizationInvitation>
{
    private OrganizationInvitationFaker()
    {
        RuleFor(i => i.Id, f => f.Random.Guid());
        RuleFor(i => i.Description, f => f.Company.CompanyName());
        RuleFor(i => i.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(i => i.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    /// <summary>
    /// Creates a fake <see cref="OrganizationInvitation"/> instance.
    /// </summary>
    /// <param name="employee">Employee to assign to the invitation.</param>
    /// <returns>Fake <see cref="OrganizationInvitation"/> instance.</returns>
    public static OrganizationInvitation Fake(Employee? employee = null)
    {
        var invitation = new OrganizationInvitationFaker().Generate();

        if (employee is not null) invitation.Employee = employee;

        return invitation;
    }
}
