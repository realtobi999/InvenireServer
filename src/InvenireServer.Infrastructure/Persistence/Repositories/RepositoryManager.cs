using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Interfaces.Repositories.Organizations;
using InvenireServer.Application.Interfaces.Repositories.Properties;
using InvenireServer.Application.Interfaces.Repositories.Users;
using InvenireServer.Domain.Exceptions.Common;
using InvenireServer.Infrastructure.Persistence.Repositories.Organizations;
using InvenireServer.Infrastructure.Persistence.Repositories.Properties;
using InvenireServer.Infrastructure.Persistence.Repositories.Users;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

/// <summary>
/// Default implementation of <see cref="IRepositoryManager"/>.
/// </summary>
public class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IAdminRepository> _admins;
    private readonly InvenireServerContext _context;
    private readonly Lazy<IEmployeeRepository> _employees;
    private readonly Lazy<IOrganizationRepository> _organizations;
    private readonly Lazy<IPropertyRepository> _properties;

    public RepositoryManager(InvenireServerContext context)
    {
        _context = context;
        _admins = new Lazy<IAdminRepository>(() => new AdminRepository(_context));
        _employees = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(_context));
        _properties = new Lazy<IPropertyRepository>(() => new PropertyRepository(_context));
        _organizations = new Lazy<IOrganizationRepository>(() => new OrganizationRepository(_context));
    }

    public IAdminRepository Admins => _admins.Value;
    public IEmployeeRepository Employees => _employees.Value;
    public IPropertyRepository Properties => _properties.Value;
    public IOrganizationRepository Organizations => _organizations.Value;

    /// <summary>
    /// Persists pending changes to the data store.
    /// </summary>
    /// <returns>Number of affected rows.</returns>
    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Persists pending changes to the data store and throws an exception if none were saved.
    /// </summary>
    /// <returns>Awaitable task representing the save operation.</returns>
    public async Task SaveOrThrowAsync()
    {
        var affected = await SaveAsync();

        if (affected == 0) throw new NoRowsAffectedException();
    }
}
