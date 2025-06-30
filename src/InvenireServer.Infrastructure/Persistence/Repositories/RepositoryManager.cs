using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Common;
using InvenireServer.Domain.Interfaces.Repositories.Organizations;
using InvenireServer.Domain.Interfaces.Repositories.Users;
using InvenireServer.Infrastructure.Persistence.Transactions;
using InvenireServer.Infrastructure.Persistence.Repositories.Organizations;
using InvenireServer.Infrastructure.Persistence.Repositories.Users;
using InvenireServer.Domain.Interfaces.Repositories.Properties;
using InvenireServer.Infrastructure.Persistence.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly InvenireServerContext _context;
    private readonly Lazy<IAdminRepository> _admins;
    private readonly Lazy<IEmployeeRepository> _employees;
    private readonly Lazy<IPropertyRepository> _properties;
    private readonly Lazy<IOrganizationRepository> _organizations;

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

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task SaveOrThrowAsync()
    {
        var affected = await SaveAsync();

        if (affected == 0) throw new NoRowsAffectedException();
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        return new Transaction(await _context.Database.BeginTransactionAsync());
    }
}