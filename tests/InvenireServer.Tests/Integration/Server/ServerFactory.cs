using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using InvenireServer.Infrastructure.Persistence;

namespace InvenireServer.Tests.Integration.Server;

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

        // Set the environment to Production.
        builder.ConfigureAppConfiguration((context, _) =>
        {
            context.HostingEnvironment.EnvironmentName = Environments.Production;
        });
    }

    /// <summary>
    /// Retrieves an instance of the <see cref="InvenireServerContext"/>.
    /// </summary>
    /// <returns>An instance of <see cref="InvenireServerContext"/>.</returns>
    public InvenireServerContext GetDatabaseContext()
    {
        var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InvenireServerContext>();
        return context;
    }
}
