using Microsoft.EntityFrameworkCore;
using InvenireServer.Tests.Integration.Fakers;
using Microsoft.Extensions.DependencyInjection;
using InvenireServer.Domain.Core.Interfaces.Email;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace InvenireServer.Tests.Integration.Server;

/// <summary>
/// Provides extension methods for configuring services during test server setup.
/// </summary>
public static class ServerFactoryExtensions
{
    /// <summary>
    /// Removes a registered service of the specified type from the service collection.
    /// </summary>
    /// <typeparam name="TService">The type of the service to remove.</typeparam>
    /// <param name="services">The service collection to modify.</param>
    private static void RemoveService<TService>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    /// <summary>
    /// Replaces the existing <see cref="IEmailSender"/> service registration with a fake implementation for testing.
    /// </summary>
    /// <param name="services">The service collection to modify.</param>
    public static void ReplaceWithFakeEmailSender(this IServiceCollection services)
    {
        services.RemoveService<IEmailSender>();
        services.AddSingleton<IEmailSender, EmailSenderFaker>();
    }

    /// <summary>
    /// Replaces the database context registration with an in-memory database configuration for testing purposes.
    /// </summary>
    /// <typeparam name="TContext">The type of the <see cref="DbContext"/> to configure.</typeparam>
    /// <param name="services">The service collection to modify.</param>
    /// <param name="dbName">The name of the in-memory database instance.</param>
    public static void ReplaceWithInMemoryDatabase<TContext>(this IServiceCollection services, string dbName) where TContext : DbContext
    {
        services.RemoveService<IDbContextOptionsConfiguration<TContext>>();
        services.AddDbContext<TContext>(options => { options.UseInMemoryDatabase(dbName); });
    }
}
