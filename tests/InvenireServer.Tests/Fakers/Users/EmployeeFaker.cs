using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Extensions;

namespace InvenireServer.Tests.Fakers.Users;

public sealed class EmployeeFaker : Faker<Employee>
{
    private EmployeeFaker()
    {
        RuleFor(e => e.Id, f => f.Random.Guid());
        RuleFor(e => e.FirstName, f => f.Name.FirstName());
        RuleFor(e => e.LastName, f => f.Name.LastName());
        RuleFor(e => e.EmailAddress, f => f.Internet.Email());
        RuleFor(e => e.IsVerified, f => f.Random.Bool());
        RuleFor(e => e.Password, f => f.Internet.SecurePassword());
        RuleFor(e => e.CreatedAt, f => f.Date.PastOffset(10));
        RuleFor(e => e.LastUpdatedAt, f => f.Date.RecentOffset(10));
        RuleFor(e => e.LastLoginAt, f => f.Date.RecentOffset(10));
    }

    public static Employee Fake(IEnumerable<PropertyItem>? items = null, IEnumerable<PropertySuggestion>? suggestions = null)
    {
        var employee = new EmployeeFaker().Generate();

        if (items is not null)
        {
            foreach (var i in items)
            {
                employee.AssignedItems.Add(i);
                i.EmployeeId = employee.Id;
            }
        }
        if (suggestions is not null)
        {
            foreach (var s in suggestions)
            {
                employee.Suggestions.Add(s);
                s.EmployeeId = employee.Id;
            }
        }

        return employee;
    }
}