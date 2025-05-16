using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace InvenireServer.Tests.Integration.Server;

public static class ServerFactoryExtensions
{
    /// <summary>
    /// Removes the specified service from the service collection.
    /// </summary>
    /// <typeparam name="TService">The type of service to remove.</typeparam>
    /// <param name="services">The service collection from which to remove the service.</param>
    public static void RemoveService<TService>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    /// <summary>
    /// Replaces the default database context with an in-memory database..
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext to replace.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="dbName">The name of the in-memory database.</param>
    public static void ReplaceWithInMemoryDatabase<TContext>(this IServiceCollection services, string dbName) where TContext : DbContext
    {
        services.RemoveService<IDbContextOptionsConfiguration<TContext>>();

        services.AddDbContext<TContext>(options => { options.UseInMemoryDatabase(dbName); });
    }
}