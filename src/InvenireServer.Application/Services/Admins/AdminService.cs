using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Admins;

namespace InvenireServer.Application.Services.Admins;

public class AdminService : IAdminService
{
    private readonly IRepositoryManager _repositories;

    public AdminService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<Admin> GetAsync(Expression<Func<Admin, bool>> predicate)
    {
        var admin = await _repositories.Admins.GetAsync(predicate);

        if (admin is null)
        {
            throw new NotFound404Exception(nameof(admin));
        }

        return admin;
    }

    public async Task CreateAsync(Admin admin)
    {
        _repositories.Admins.Create(admin);
        await _repositories.SaveOrThrowAsync();
    }
}
