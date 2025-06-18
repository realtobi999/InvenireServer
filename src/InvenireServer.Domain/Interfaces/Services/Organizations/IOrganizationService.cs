using System.Linq.Expressions;
using InvenireServer.Domain.Entities;

namespace InvenireServer.Domain.Interfaces.Services.Organizations;

public interface IOrganizationService
{
    Task<Organization> GetAsync(Expression<Func<Organization, bool>> predicate);

    Task CreateAsync(Organization organization);
}