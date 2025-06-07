using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Common;
using InvenireServer.Domain.Interfaces.Repositories;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IAdminRepository> _admins;
    private readonly InvenireServerContext _context;
    private readonly Lazy<IEmployeeRepository> _employees;

    public RepositoryManager(InvenireServerContext context)
    {
        _context = context;
        _admins = new Lazy<IAdminRepository>(() => new AdminRepository(_context));
        _employees = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(_context));
    }

    public IAdminRepository Admins => _admins.Value;

    public IEmployeeRepository Employees => _employees.Value;

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task SaveOrThrowAsync()
    {
        var affected = await SaveAsync();

        if (affected == 0) throw new NoRowsAffectedException();
    }
}