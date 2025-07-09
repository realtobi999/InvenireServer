using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Admins;
using InvenireServer.Domain.Validators.Users;

namespace InvenireServer.Application.Services.Admins;

public class AdminService : IAdminService
{
    private readonly IRepositoryManager _repositories;

    public AdminService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public Task<IEnumerable<Admin>> IndexInactiveAsync()
    {
        return _repositories.Admins.IndexInactiveAsync();
    }

    public async Task<Admin> GetAsync(Jwt jwt)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "admin_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null) throw new BadRequest400Exception("Missing or invalid 'admin_id' claim.");

        if (!Guid.TryParse(claim.Value, out var id)) throw new BadRequest400Exception("Invalid format for 'admin_id' claim.");

        return await GetAsync(e => e.Id == id);
    }

    public async Task<Admin> GetAsync(Expression<Func<Admin, bool>> predicate)
    {
        var admin = await _repositories.Admins.GetAsync(predicate);

        if (admin is null) throw new NotFound404Exception($"The requested {nameof(Admin).ToLower()} was not found in the system");

        return admin;
    }

    public async Task CreateAsync(Admin admin)
    {
        var result = new ValidationResult(AdminEntityValidator.Validate(admin));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(Admin).ToLower()} (ID: {admin.Id}).", result.Errors);

        _repositories.Admins.Create(admin);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task UpdateAsync(Admin admin)
    {
        admin.LastUpdatedAt = DateTimeOffset.UtcNow;

        var result = new ValidationResult(AdminEntityValidator.Validate(admin));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(Admin).ToLower()} (ID: {admin.Id}).", result.Errors);

        _repositories.Admins.Update(admin);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task DeleteAsync(IEnumerable<Admin> admins)
    {
        foreach (var admin in admins) _repositories.Admins.Delete(admin);
        await _repositories.SaveOrThrowAsync();
    }
}