using System.Security.Claims;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Core.Admins.Commands.Login;

/// <summary>
/// Handler for the request to authenticate an admin.
/// </summary>
public class LoginAdminCommandHandler : IRequestHandler<LoginAdminCommand, LoginAdminCommandResult>
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

    /// <summary>
    /// Handles the request to authenticate an admin.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
    public async Task<LoginAdminCommandResult> Handle(LoginAdminCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(a => a.EmailAddress == request.EmailAddress) ?? throw new Unauthorized401Exception("Invalid credentials.");

        if (!admin.IsVerified) throw new Unauthorized401Exception("Verification is required to proceed with login.");
        if (_hasher.VerifyHashedPassword(admin, admin.Password, request.Password) == PasswordVerificationResult.Failed) throw new Unauthorized401Exception("Invalid credentials.");

        admin.LastLoginAt = DateTimeOffset.UtcNow;
        await _repositories.Admins.ExecuteUpdateAsync(admin);

        var token = _jwt.Builder.Build(
        [
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]);
        return new LoginAdminCommandResult
        {
            Token = token,
            TokenString = _jwt.Writer.Write(token)
        };
    }
}