using InvenireServer.Domain.Core.Interfaces.Common;
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
    private static void RemoveService<TService>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    /// <summary>
    /// Replaces the existing <see cref="IEmailSender"/> service registration with the specified implementation.
    /// </summary>
    /// <typeparam name="TEmailSender">The implementation type of <see cref="IEmailSender"/> to register.</typeparam>
    public static void ReplaceEmailSender<TEmailSender>(this IServiceCollection services) where TEmailSender : class, IEmailSender
    {
        services.RemoveService<IEmailSender>();
        services.AddScoped<IEmailSender, TEmailSender>();
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