using System.Security.Claims;
using InvenireServer.Presentation;
using Microsoft.Extensions.Configuration;
using InvenireServer.Domain.Core.Options;
using InvenireServer.Application.Core.Authentication;

namespace InvenireServer.Tests.Integration.Fakers;

public class JwtManagerFaker
{
    public JwtManager Initiate()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddUserSecrets<Program>()
            .Build();

        var options = configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new NullReferenceException("JWT Configuration is missing or incomplete.");
        var manager = new JwtManager(options);

        return manager;
    }
}
