using System.Linq.Expressions;
using System.Security.Claims;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Admins;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Application.Services.Admins;

public class AdminService : IAdminService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailManager _email;
    private readonly IJwtManager _jwt;
    private readonly IRepositoryManager _repositories;

    public AdminService(IRepositoryManager repositories, IEmailManager email, IJwtManager jwt, IConfiguration configuration)
    {
        _jwt = jwt;
        _email = email;
        _repositories = repositories;
        _configuration = configuration;
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

        if (admin is null) throw new NotFound404Exception($"The requested {nameof(admin)} was not found in the system");

        return admin;
    }

    public async Task CreateAsync(Admin admin)
    {
        _repositories.Admins.Create(admin);
        await _repositories.SaveOrThrowAsync();
    }

    public Jwt CreateJwt(Admin admin)
    {
        return _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", admin.IsVerified ? bool.TrueString : bool.FalseString)
        ]);
    }

    public async Task UpdateAsync(Admin admin)
    {
        admin.LastUpdatedAt = DateTimeOffset.UtcNow;

        _repositories.Admins.Update(admin);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task SendEmailVerificationAsync(Admin admin)
    {
        var jwt = CreateJwt(admin);

        jwt.Payload.Add(new Claim("purpose", "email_verification"));

        var dto = new AdminVerificationEmailDto
        {
            AdminAddress = admin.EmailAddress,
            AdminName = admin.Name,
            VerificationLink = $"{_configuration.GetSection("Frontend:BaseUrl").Value ?? throw new NullReferenceException()}/verify-email?token={_jwt.Writer.Write(jwt)}"
        };

        var message = _email.Builders.Admin.BuildVerificationEmail(dto);
        await _email.Sender.SendEmailAsync(message);
    }

    public async Task ConfirmEmailVerificationAsync(Admin admin)
    {
        if (admin.IsVerified) throw new BadRequest400Exception("Email is already verified.");

        admin.IsVerified = true;
        await UpdateAsync(admin);
    }
}