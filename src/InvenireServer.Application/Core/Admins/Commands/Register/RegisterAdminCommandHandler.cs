using System.Security.Claims;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Core.Admins.Commands.Register;

public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, RegisterAdminCommandResult>
{
    private readonly IJwtManager _jwt;
    private readonly IServiceManager _services;
    private readonly IPasswordHasher<Admin> _hasher;

    public RegisterAdminCommandHandler(IServiceManager services, IPasswordHasher<Admin> hasher, IJwtManager jwt)
    {
        _jwt = jwt;
        _hasher = hasher;
        _services = services;
    }

    public async Task<RegisterAdminCommandResult> Handle(RegisterAdminCommand request, CancellationToken _)
    {
        // Build the admin from the request and hash the password.
        var admin = new Admin
        {
            Id = request.Id ?? Guid.NewGuid(),
            Name = request.Name,
            EmailAddress = request.EmailAddress,
            Password = request.Password,
            IsVerified = false,
            CreatedAt = DateTimeOffset.UtcNow,
            LastLoginAt = null,
            LastUpdatedAt = null,
        };
        admin.Password = _hasher.HashPassword(admin, admin.Password);

        // Save the admin to the database.
        await _services.Admins.CreateAsync(admin);

        // Create the jwt.
        var jwt = _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]);

        return new RegisterAdminCommandResult
        {
            Admin = admin,
            Token = _jwt.Writer.Write(jwt),
        };
    }
}
