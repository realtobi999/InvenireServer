using InvenireServer.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InvenireServer.Tests.Integration.Server;

public class ServerFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithFakeEmailSender();
            services.ReplaceWithInMemoryDatabase<InvenireServerContext>(_dbName);
        });

        // Set the hosting environment to Production to simulate production behavior in tests.
        builder.ConfigureAppConfiguration((context, _) => { context.HostingEnvironment.EnvironmentName = Environments.Production; });
    }

    public InvenireServerContext GetDatabaseContext()
    {
        var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InvenireServerContext>();
        return context;
    }
}