using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Infrastructure.Authentication.Options;
using InvenireServer.Presentation;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Tests.Fakers.Common;

public class JwtManagerFaker
{
    public static JwtManager Initiate()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
            .AddUserSecrets<Program>()
            .Build();

        var options = configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new NullReferenceException("JWT Configuration is missing or incomplete.");
        var manager = new JwtManager(options);

        return manager;
    }
}