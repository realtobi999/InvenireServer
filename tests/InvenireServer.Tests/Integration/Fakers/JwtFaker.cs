using System.Security.Claims;
using InvenireServer.Presentation;
using Microsoft.Extensions.Configuration;
using InvenireServer.Application.Core.Factories;
using InvenireServer.Domain.Core.Entities.Common;

namespace InvenireServer.Tests.Integration.Fakers;

/// <summary>
/// Provides methods to create JWT tokens for testing purposes using configured settings.
/// </summary>
public class JwtFaker
{
    /// <summary>
    /// Creates a JWT token with the specified claims as payload, using the application configuration and JWT factory settings.
    /// </summary>
    /// <param name="payload">The collection of claims to include in the JWT payload.</param>
    /// <returns>A JWT token generated with the given claims.</returns>
    public Jwt Create(IEnumerable<Claim> payload)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddUserSecrets<Program>()
            .Build();

        var factory = new JwtFactory(configuration);

        return factory.Create(payload);
    }
}
