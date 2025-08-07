using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Tests.Fakers.Organizations;

public sealed class OrganizationFaker : Faker<Organization>
{
    private OrganizationFaker()
    {
        RuleFor(o => o.Id, f => f.Random.Guid());
        RuleFor(o => o.Name, f => f.Company.CompanyName());
        RuleFor(o => o.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(o => o.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    public static Organization Fake(Admin? admin = null, Property? property = null, List<Employee>? employees = null, List<OrganizationInvitation>? invitations = null)
    {
        var organization = new OrganizationFaker().Generate();

        if (admin is not null) organization.Admin = admin;
        if (property is not null) organization.AssignProperty(property);
        if (employees is not null) organization.AddEmployees(employees);
        if (invitations is not null) organization.AddInvitations(invitations);

        return organization;
    }
}