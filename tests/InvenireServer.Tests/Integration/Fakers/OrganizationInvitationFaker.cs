using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Tests.Integration.Fakers;

public class OrganizationInvitationFaker : Faker<OrganizationInvitation>
{
    public OrganizationInvitationFaker()
    {
        RuleFor(i => i.Id, f => f.Random.Guid());
        RuleFor(i => i.Description, f => f.Company.CompanyName());
        RuleFor(i => i.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(i => i.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    public OrganizationInvitationFaker(Organization organization, Employee employee) : this()
    {
        RuleFor(i => i.Employee, _ => employee);
        RuleFor(i => i.OrganizationId, _ => organization.Id);
    }
}