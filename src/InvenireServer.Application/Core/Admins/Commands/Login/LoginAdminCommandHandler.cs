using System.Security.Claims;
using System.Transactions;
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

    public async Task<LoginAdminCommandResult> Handle(LoginAdminCommand request, CancellationToken _)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // Retrieves the admin and returns 401 unauthorized if the admin is not found.
        Admin admin;
        try
        {
            admin = await _services.Admins.GetAsync(e => e.EmailAddress == request.EmailAddress);
        }
        catch (NotFound404Exception)
        {
            throw new Unauthorized401Exception("Invalid credentials.");
        }

        // Make sure that the admin is verified before logging in.
        if (!admin.IsVerified) throw new Unauthorized401Exception("Verification required to proceed.");

        // Validate the provided credentials.
        if (_hasher.VerifyHashedPassword(admin, admin.Password, request.Password) == PasswordVerificationResult.Failed) throw new Unauthorized401Exception("Invalid credentials.");

        // Update the timestamp of the admins's last login.
        admin.LastLoginAt = DateTimeOffset.Now;
        await _services.Admins.UpdateAsync(admin);

        // Create the jwt.
        var jwt = _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]);

        scope.Complete();

        return new LoginAdminCommandResult
        {
            Token = _jwt.Writer.Write(jwt)
        };
    }
}