using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Admins.Commands.Register;

public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, RegisterAdminCommandResult>
{
    private readonly IJwtManager _jwt;
    private readonly IRepositoryManager _repositories;
    private readonly IPasswordHasher<Admin> _hasher;

    public RegisterAdminCommandHandler(IJwtManager jwt, IPasswordHasher<Admin> hasher, IRepositoryManager repositories)
    {
        _jwt = jwt;
        _hasher = hasher;
        _repositories = repositories;
    }

    public async Task<RegisterAdminCommandResult> Handle(RegisterAdminCommand request, CancellationToken ct)
    {
        var admin = new Admin
        {
            Id = request.Id ?? Guid.NewGuid(),
            Name = request.Name,
            EmailAddress = request.EmailAddress,
            Password = request.Password,
            IsVerified = false,
            CreatedAt = DateTimeOffset.UtcNow,
            LastLoginAt = null,
            LastUpdatedAt = null
        };
        admin.Password = _hasher.HashPassword(admin, admin.Password);

        _repositories.Admins.Create(admin);

        await _repositories.SaveOrThrowAsync();

        var token = _jwt.Builder.Build(
            [
                new Claim("role", Jwt.Roles.ADMIN),
                new Claim("admin_id", admin.Id.ToString()),
                new Claim("is_verified", bool.FalseString)
            ]);
        return new RegisterAdminCommandResult
        {
            Admin = admin,
            Token = token,
            TokenString = _jwt.Writer.Write(token)
        };
    }
}