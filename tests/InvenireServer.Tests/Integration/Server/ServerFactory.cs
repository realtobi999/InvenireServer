using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using InvenireServer.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InvenireServer.Tests.Integration.Server;

/// <summary>
/// Provides a test server factory for integration tests.
/// </summary>
public class ServerFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    /// <summary>
    /// Configures the web host for the test server.
    /// </summary>
    /// <param name="builder">Web host builder.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IHostedService>();
            services.ReplaceWithFakeEmailSender();
            services.ReplaceWithInMemoryDatabase<InvenireServerContext>(Guid.NewGuid().ToString());
        });

        // Set the hosting environment to Production to simulate production behavior in tests.
        builder.ConfigureAppConfiguration((context, _) => { context.HostingEnvironment.EnvironmentName = Environments.Production; });
    }

    /// <summary>
    /// Gets a scoped database context for the test server.
    /// </summary>
    /// <returns>Database context.</returns>
    public InvenireServerContext GetDatabaseContext()
    {
        var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InvenireServerContext>();
        return context;
    }
}
