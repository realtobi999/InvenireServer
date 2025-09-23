using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using InvenireServer.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InvenireServer.Tests.Integration.Server;

public class ServerFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IHostedService>();
            services.ReplaceWithFakeEmailSender();
            services.ReplaceWithInMemoryDatabase<InvenireServerContext>();
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