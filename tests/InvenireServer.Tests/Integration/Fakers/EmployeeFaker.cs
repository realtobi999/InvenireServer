using InvenireServer.Domain.Core.Entities;
using InvenireServer.Tests.Integration.Extensions;

namespace InvenireServer.Tests.Integration.Fakers;

public sealed class EmployeeFaker : Faker<Employee>
{
    public EmployeeFaker()
    {
        RuleFor(e => e.Id, f => f.Random.Guid());
        RuleFor(e => e.Name, f => f.Name.FullName());
        RuleFor(e => e.EmailAddress, f => f.Internet.Email());
        RuleFor(e => e.Password, f => f.Internet.SecurePassword());
        RuleFor(e => e.UpdatedAt, f => f.Date.RecentOffset(10));
        RuleFor(e => e.CreatedAt, f => f.Date.PastOffset(10));
        RuleFor(e => e.OrganizationId, f => f.Random.Bool() ? f.Random.Guid() : null); // TODO: Change this to be a id of the organization passed in the constructor.
    }
}
