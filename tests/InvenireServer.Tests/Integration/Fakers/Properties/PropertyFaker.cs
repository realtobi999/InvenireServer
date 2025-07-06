using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Integration.Fakers.Properties;

public class PropertyFaker : Faker<Property>
{
    public PropertyFaker()
    {
        RuleFor(p => p.Id, f => f.Random.Guid());
        RuleFor(p => p.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(p => p.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    public PropertyFaker(Organization organization) : this()
    {
        RuleFor(p => p.OrganizationId, _ => organization.Id);
    }
}