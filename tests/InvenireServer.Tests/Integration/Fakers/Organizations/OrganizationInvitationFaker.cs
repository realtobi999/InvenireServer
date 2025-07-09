using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Tests.Integration.Fakers.Organizations;

public class OrganizationInvitationFaker : Faker<OrganizationInvitation>
{
    private OrganizationInvitationFaker()
    {
        RuleFor(i => i.Id, f => f.Random.Guid());
        RuleFor(i => i.Description, f => f.Company.CompanyName());
        RuleFor(i => i.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(i => i.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    public static OrganizationInvitation Fake(Employee? employee = null)
    {
        var invitation = new OrganizationInvitationFaker().Generate();

        if (employee is not null) invitation.AssignEmployee(employee);

        return invitation;
    }
}