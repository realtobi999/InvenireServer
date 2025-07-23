using System.Security.Claims;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Core.Admins.Commands.Login;

public class LoginAdminCommandHandler : IRequestHandler<LoginAdminCommand, LoginAdminCommandResult>
{
    private readonly IPasswordHasher<Admin> _hasher;
    private readonly IJwtManager _jwt;
    private readonly IServiceManager _services;

    public LoginAdminCommandHandler(IServiceManager services, IPasswordHasher<Admin> hasher, IJwtManager jwt)
    {
        _jwt = jwt;
        _hasher = hasher;
        _services = services;
    }

    public async Task<LoginAdminCommandResult> Handle(LoginAdminCommand request, CancellationToken ct)
    {
        // Verify the  email  belongs  to  a  verified  admin  and  confirm  the
        // password; return unauthorized if any check fails.
        var admin = (Admin?)null;
        try
        {
            admin = await _services.Admins.GetAsync(e => e.EmailAddress == request.EmailAddress);
        }
        catch (NotFound404Exception)
        {
            throw new Unauthorized401Exception("Invalid credentials.");
        }
        if (!admin.IsVerified) throw new Unauthorized401Exception("Verification is required to proceed with login.");
        if (_hasher.VerifyHashedPassword(admin, admin.Password, request.Password) == PasswordVerificationResult.Failed) throw new Unauthorized401Exception("Invalid credentials.");

        admin.LastLoginAt = DateTimeOffset.UtcNow;

        await _services.Admins.UpdateAsync(admin);

        return new LoginAdminCommandResult
        {
            Token = _jwt.Writer.Write(_jwt.Builder.Build([
                new Claim("role", Jwt.Roles.ADMIN),
                new Claim("admin_id", admin.Id.ToString()),
                new Claim("is_verified", bool.TrueString)
            ]))
        };
    }
}