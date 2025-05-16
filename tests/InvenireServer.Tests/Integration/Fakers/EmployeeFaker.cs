using System.ComponentModel.DataAnnotations;
using Bogus;
using InvenireServer.Domain.Core.Dtos.Common;
using InvenireServer.Domain.Core.Entities;

namespace InvenireServer.Tests.Integration.Fakers;

public class EmployeeFaker : Faker<Employee>
{
    public EmployeeFaker()
    {
        RuleFor(e => e.Id, f => f.Random.Guid());
        RuleFor(e => e.Name, f => f.Name.FullName());
        RuleFor(e => e.EmailAddress, f => f.Internet.Email());
        RuleFor(e => e.Password, f => f.Internet.Password());
        RuleFor(e => e.LoginAttempts, f => 0);
        RuleFor(e => e.LoginLock, f => new LoginLockDto
        {
            IsSet = false,
            ExpirationDate = f.Date.FutureOffset()
        });
        RuleFor(e => e.UpdatedAt, f => f.Date.RecentOffset(10));
        RuleFor(e => e.CreatedAt, f => f.Date.PastOffset(10));
        RuleFor(e => e.OrganizationId, f => f.Random.Bool() ? f.Random.Guid() : null);
    }
}
