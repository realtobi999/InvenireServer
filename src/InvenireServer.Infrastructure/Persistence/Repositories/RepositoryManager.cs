using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Common;
using InvenireServer.Domain.Interfaces.Repositories;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides access to repository implementations and database persistence operations.
/// </summary>
public class RepositoryManager : IRepositoryManager
{
    private readonly InvenireServerContext _context;
    private readonly Lazy<IAdminRepository> _admins;
    private readonly Lazy<IEmployeeRepository> _employees;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryManager"/> class using the specified database context.
    /// </summary>
    /// <param name="context">The database context used by the repositories.</param>
    public RepositoryManager(InvenireServerContext context)
    {
        _context = context;
        _admins = new Lazy<IAdminRepository>(() => new AdminRepository(_context));
        _employees = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(_context));
    }

    /// <inheritdoc/>
    public IAdminRepository Admins => _admins.Value;

    /// <inheritdoc/>
    public IEmployeeRepository Employees => _employees.Value;

    /// <inheritdoc/>
    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Saves all changes to the database. Throws if no entries were affected.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    /// <exception cref="NoRowsAffectedException">Thrown when no rows are affected by the save operation.</exception>
    public async Task SaveOrThrowAsync()
    {
        var affected = await SaveAsync();

        if (affected == 0)
        {
            throw new NoRowsAffectedException();
        }
    }
}