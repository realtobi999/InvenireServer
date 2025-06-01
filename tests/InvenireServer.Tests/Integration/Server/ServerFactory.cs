using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using InvenireServer.Infrastructure.Persistence;

namespace InvenireServer.Tests.Integration.Server;

/// <summary>
/// Provides a test server factory with customized configuration for integration testing.
/// </summary>
/// <typeparam name="TStartup">The type of the startup class to configure the test server.</typeparam>
public class ServerFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithFakeEmailSender();
            services.ReplaceWithInMemoryDatabase<InvenireServerContext>(_dbName);
        });

        // Set the hosting environment to Production to simulate production behavior in tests.
        builder.ConfigureAppConfiguration((context, _) =>
        {
            context.HostingEnvironment.EnvironmentName = Environments.Production;
        });
    }

    /// <summary>
    /// Creates a scoped service provider and retrieves an instance of the <see cref="InvenireServerContext"/>.
    /// </summary>
    /// <returns>An instance of <see cref="InvenireServerContext"/>.</returns>
    public InvenireServerContext GetDatabaseContext()
    {
        var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InvenireServerContext>();
        return context;
    }
}
