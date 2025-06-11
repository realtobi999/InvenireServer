using InvenireServer.Domain.Entities;
using InvenireServer.Tests.Integration.Extensions;

namespace InvenireServer.Tests.Integration.Fakers;

public class AdminFaker : Faker<Admin>
{
    public AdminFaker()
    {
        RuleFor(e => e.Id, f => f.Random.Guid());
        RuleFor(e => e.Name, f => f.Name.FullName());
        RuleFor(e => e.EmailAddress, f => f.Internet.Email());
        RuleFor(e => e.IsVerified, f => f.Random.Bool());
        RuleFor(e => e.Password, f => f.Internet.SecurePassword());
        RuleFor(e => e.CreatedAt, f => f.Date.PastOffset(10));
        RuleFor(e => e.LastUpdatedAt, f => f.Date.RecentOffset(10));
        RuleFor(e => e.LastLoginAt, f => f.Date.RecentOffset(10));
    }

    public AdminFaker(Organization organization) : this()
    {
        RuleFor(e => e.OrganizationId, _ => organization.Id);
    }
}
