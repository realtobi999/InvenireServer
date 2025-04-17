using InvenireServer.Domain.Core.Exceptions.Common;
using InvenireServer.Domain.Core.Interfaces.Managers;

namespace InvenireServer.Infrastructure.Persistence.Managers;

public class RepositoryManager : IRepositoryManager
{
    private readonly InvenireServerContext _context;

    public RepositoryManager(InvenireServerContext context)
    {
        _context = context;
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Saves all changes to the database. Throws an exception if no entries were affected.
    /// </summary>
    /// <exception cref="ZeroRowsAffectedException"> Thrown when the save operation completes without affecting any entries. </exception>
    public async Task SaveOrThrowAsync()
    {
        var affected = await SaveAsync();

        if (affected == 0)
        {
            throw new ZeroRowsAffectedException();
        }
    }
}
