using System.Security.Claims;
using InvenireServer.Application.Core.Factories;
using InvenireServer.Domain.Core.Entities.Common;
using InvenireServer.Presentation;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Tests.Integration.Fakers;

public static class JwtFaker
{
    /// <summary>
    /// Creates a JWT token using the provided payload and configuration settings from Presentation layer.
    /// </summary>
    /// <param name="payload">The claims to include in the JWT payload.</param>
    /// <returns>A JWT object containing the header, payload, and signature.</returns>
    public static Jwt Create(IEnumerable<Claim> payload)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                      .AddJsonFile("appsettings.json")
                                                      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                                                      .AddUserSecrets<Program>().Build();

        var factory = new JwtFactory(configuration);

        return factory.Create(payload);
    }
}
