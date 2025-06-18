using InvenireServer.Domain.Entities;

namespace InvenireServer.Tests.Integration.Fakers;

public class OrganizationFaker : Faker<Organization>
{
    public OrganizationFaker()
    {
        RuleFor(o => o.Id, f => f.Random.Guid());
        RuleFor(o => o.Name, f => f.Company.CompanyName());
        RuleFor(o => o.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(o => o.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }
}