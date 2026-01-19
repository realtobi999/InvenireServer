using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Extensions;

namespace InvenireServer.Tests.Fakers.Users;

/// <summary>
/// Provides fake <see cref="Admin"/> instances for tests.
/// </summary>
public sealed class AdminFaker : Faker<Admin>
{
    private AdminFaker()
    {
        RuleFor(a => a.Id, f => f.Random.Guid());
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
        RuleFor(a => a.EmailAddress, f => f.Internet.Email());
        RuleFor(a => a.IsVerified, f => f.Random.Bool());
        RuleFor(a => a.Password, f => f.Internet.SecurePassword());
        RuleFor(a => a.CreatedAt, f => f.Date.PastOffset(10));
        RuleFor(a => a.LastUpdatedAt, f => f.Date.RecentOffset(10));
        RuleFor(a => a.LastLoginAt, f => f.Date.RecentOffset(10));
    }

    /// <summary>
    /// Creates a fake <see cref="Admin"/> instance.
    /// </summary>
    /// <returns>Fake <see cref="Admin"/> instance.</returns>
    public static Admin Fake()
    {
        return new AdminFaker().Generate();
    }
}
