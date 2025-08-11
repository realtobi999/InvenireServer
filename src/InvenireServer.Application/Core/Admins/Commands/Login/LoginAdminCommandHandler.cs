using System.Security.Claims;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Core.Admins.Commands.Login;

public class LoginAdminCommandHandler : IRequestHandler<LoginAdminCommand, LoginAdminCommandResponse>
{
    private readonly IJwtManager _jwt;
    private readonly IRepositoryManager _repositories;
    private readonly IPasswordHasher<Admin> _hasher;

    public LoginAdminCommandHandler(IJwtManager jwt, IPasswordHasher<Admin> hasher, IRepositoryManager repositories)
    {
        _jwt = jwt;
        _hasher = hasher;
        _repositories = repositories;
    }

    public async Task<LoginAdminCommandResponse> Handle(LoginAdminCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(a => a.EmailAddress == request.EmailAddress) ?? throw new Unauthorized401Exception("Invalid credentials.");

        if (!admin.IsVerified) throw new Unauthorized401Exception("Verification is required to proceed with login.");
        if (_hasher.VerifyHashedPassword(admin, admin.Password, request.Password) == PasswordVerificationResult.Failed) throw new Unauthorized401Exception("Invalid credentials.");

        admin.LastLoginAt = DateTimeOffset.UtcNow;

        _repositories.Admins.Update(admin);

        await _repositories.SaveOrThrowAsync();

        return new LoginAdminCommandResponse
        {
            Token = _jwt.Writer.Write(_jwt.Builder.Build([
                new Claim("role", Jwt.Roles.ADMIN),
                new Claim("admin_id", admin.Id.ToString()),
                new Claim("is_verified", bool.TrueString)
            ]))
        };
    }
}